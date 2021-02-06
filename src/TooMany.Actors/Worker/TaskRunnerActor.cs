using System;
using System.Linq;
using System.Threading.Tasks;
using K4os.Json.Messages.Interfaces;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Persistence;
using TooMany.Actors.Messages;
using TooMany.Actors.Tools;
using TooMany.Actors.Worker.Processes;
using TooMany.Messages;

namespace TooMany.Actors.Worker
{
	public class TaskRunnerActor: PersistentActor<TaskDefinition>, IActor
	{
		public const string ActorName = "TaskRunner";

		private static readonly StringComparer? StringComparer =
			StringComparer.InvariantCultureIgnoreCase;

		private readonly SlidingLog _slidingLog;
		private readonly IRealtimeService _realtimeService;
		private readonly IProcessFactory _processFactory;

		private TaskDefinition? _definition;

		private TaskState _actualState = TaskState.Stopped;
		private DateTime? _startedTime;
		private bool _rebootRequired;
		private bool _syncInProgress;
		private Guid _recentSyncId;

		private IProcessSupervisor? _supervisor;

		private string TaskId => _definition?.Name ?? "<unknown>";

		public TaskRunnerActor(
			ILoggerFactory loggerFactory,
			IProvider persistenceProvider,
			IRealtimeService realtimeService,
			IProcessFactory processFactory):
			base(loggerFactory, persistenceProvider)
		{
			_realtimeService = realtimeService;
			_processFactory = processFactory;
			_slidingLog = new SlidingLog(1000);
		}

		public Task ReceiveAsync(IContext context) =>
			context.Message switch {
				Started _ => OnStarted(context),
				Stopping _ => OnStopping(context),
				// ----
				DefineTask m => OnDefineTask(context, m),
				RemoveTask m => OnRemoveTask(context, m),
				SetTags m => OnSetTags(context, m),
				// ----
				GetTask m => OnGetTask(context, m),
				StartTask m => OnStartTask(context, m),
				StopTask m => OnStopTask(context, m),
				// ----
				LogEntry e => OnLogAdded(e),
				GetLog m => OnGetLog(context, m),
				SyncState m => OnSyncState(context, m),
				// ----
				_ => Task.CompletedTask,
			};

		private async Task OnLogAdded(LogEntry entry)
		{
			_slidingLog.Add(entry);
			await SendRealtime(entry);
		}

		private Task OnGetLog(IContext context, GetLog request)
		{
			var messages = _slidingLog.Snapshot();
			context.Respond(new TaskLog(request, messages));
			return Task.CompletedTask;
		}

		private async Task OnStartTask(IContext context, StartTask request)
		{
			var force = request.Force ?? false;
			var isRunning = _actualState == TaskState.Started;
			if (isRunning && force) _rebootRequired = true;
			await UpdateState(context, request, TaskState.Started);
		}

		private Task OnStopTask(IContext context, StopTask request)
		{
			_rebootRequired = true;
			return UpdateState(context, request, TaskState.Stopped);
		}

		private Task OnSyncState(IContext context, SyncState message)
		{
			if (message.SyncId != _recentSyncId)
				return Task.CompletedTask;
			
			if (_syncInProgress)
			{
				ScheduleSync(context);
				return Task.CompletedTask;
			}

			var expectedState = _definition?.ExpectedState ?? TaskState.Stopped;
			var actualState = _actualState;

			var isRunning = actualState == TaskState.Started;
			var shouldBeRunning = expectedState == TaskState.Started && !_rebootRequired;

			if (_definition is null || isRunning == shouldBeRunning)
				return Task.CompletedTask;

			return
				shouldBeRunning ? StartProcess(context, _definition) :
				isRunning ? StopProcess(context) :
				Task.CompletedTask;
		}
		
		private static Action<LogEntry> CreateLogAction(IInfoContext context)
		{
			var root = context.System.Root;
			var self = context.Self!;
			return e => root.Send(self, e);
		}

		public Task StartProcess(IContext context, TaskDefinition definition)
		{
			_syncInProgress = true;
			
			var supervisor = _processFactory.Create(definition, CreateLogAction(context));

			context.ReenterAfter(
				TryStartProcess(supervisor),
				result => OnProcessStartedOrFailed(context, supervisor, result.Exception));

			return Task.CompletedTask;
		}

		private Task<Exception?> TryStartProcess(IProcessSupervisor supervisor)
		{
			Log.LogInformation($"Starting '{TaskId}'");
			return supervisor.Start();
		}

		private Task StopProcess(IContext context)
		{
			if (_supervisor is null)
				return Task.CompletedTask;

			_syncInProgress = true;

			context.ReenterAfter(
				TryStopProcess(_supervisor),
				result => OnProcessKilledOrNot(context, result.Result));
			
			return Task.CompletedTask;
		}

		private async Task<bool> TryStopProcess(IProcessSupervisor supervisor)
		{
			Log.LogInformation($"Stopping '{TaskId}'");
			if (await supervisor.Stop()) return true;

			Log.LogWarning($"Killing '{TaskId}'");
			if (await supervisor.Kill()) return true;

			return false;
		}

		private Task OnProcessStartedOrFailed(
			IContext context, IProcessSupervisor supervisor, Exception? exception)
		{
			_syncInProgress = false;
			ScheduleSync(context);
			
			return exception switch {
				null => OnProcessStarted(context, supervisor),
				_ => OnProcessFailed(context, exception.Unwrap()),
			};
		}
		
		private Task OnProcessKilledOrNot(IContext context, bool stopped)
		{
			_syncInProgress = false;
			ScheduleSync(context);
			
			if (!stopped)
				Log.LogError($"Failed to stop '{TaskId}', scheduling retry...");
			
			return Task.CompletedTask;
		}

		private Task OnProcessStarted(IContext context, IProcessSupervisor supervisor)
		{
			Log.LogInformation($"Task '{TaskId}' started");
			context.ReenterAfter(supervisor.Wait(), t => OnProcessStopped(context, t.Result));
			_actualState = TaskState.Started;
			_startedTime = DateTime.UtcNow;
			_supervisor = supervisor;
			return Notify2(context, ToTaskSnapshot());
		}

		private Task OnProcessFailed(IContext context, Exception exception)
		{
			Log.LogError(exception, $"Task '{TaskId}' failed to start");
			ScheduleSync(context);
			_actualState = TaskState.Failed;
			_startedTime = null;
			_rebootRequired = false;
			_supervisor = null;
			return Notify2(context, ToTaskSnapshot());
		}

		private Task OnProcessStopped(IContext context, int exitCode)
		{
			var level = exitCode == 0 ? LogLevel.Information : LogLevel.Warning;
			Log.Log(level, $"Task '{TaskId}' finished with exit code {exitCode}");
			ScheduleSync(context);
			var gracefulOrExpected =
				exitCode == 0 || // graceful 
				_definition is { ExpectedState: TaskState.Stopped }; // expected

			_actualState = gracefulOrExpected ? TaskState.Stopped : TaskState.Failed;
			_startedTime = null;
			_rebootRequired = false;
			_supervisor = null;
			return Notify2(context, ToTaskSnapshot());
		}
		
		private void ScheduleSync(IContext context)
		{
			var syncId = Guid.NewGuid();
			_recentSyncId = syncId;
			context.SendLater(context.Self!, TimeSpan.FromSeconds(1), () => new SyncState(syncId));
		}

		private async Task OnStarted(IContext context)
		{
			await RestoreState(context);
			ScheduleSync(context);
		}

		private Task OnStopping(IContext context) =>
			_actualState != TaskState.Started
				? Task.CompletedTask
				: StopProcess(context);

		private Task OnGetTask(IContext context, GetTask request) =>
			_definition is null
				? context.Return(ToTaskNotFound(request))
				: context.Return(ToTaskSnapshot(request));

		private async Task OnDefineTask(IContext context, DefineTask request)
		{
			var id = context.Self!.Id;
			var created = _definition is null;
			_definition = NewDefinition(request);
			await Persist();
			await (created
				? Respond3(context, ToTaskCreated(request, id))
				: Respond3(context, ToTaskUpdated(request, id)));

			await StopProcess(context);
		}

		private async Task OnSetTags(IContext context, SetTags request)
		{
			var error = Validate(request);
			if (error != null)
			{
				context.Respond(error);
				return;
			}

			_definition!.Tags = request.Tags
				.Distinct(StringComparer)
				.OrderBy(x => x, StringComparer)
				.ToList();

			await Persist();

			await Respond3(context, ToTaskSnapshot(request));
		}

		private IError? Validate(SetTags request)
		{
			if (_definition is null)
				return new TaskNotFound(request, request.Name);

			var invalidTags = Tags.InvalidTags(request.Tags);
			if (invalidTags != null)
				return ToBadRequest(request, invalidTags);

			return null;
		}

		private async Task UpdateState(
			IContext context, IRequest request, TaskState state)
		{
			if (_definition is null) return;

			_definition.ExpectedState = state;
			
			await Persist();
			ScheduleSync(context);

			await Respond3(context, ToTaskSnapshot(request));
		}

		private Task OnRemoveTask(IContext context, RemoveTask request)
		{
			var id = context.Self!.Id;
			context.Stop(context.Self!);
			var definition = _definition ?? NewDefinition(request);
			return Respond3(context, ToTaskRemoved(request, id, definition));
		}

		private async Task<bool> Respond3<T>(IContext context, T snapshot) where T: TaskSnapshot
		{
			await Notify2(context, snapshot);
			context.Respond(snapshot);
			return true;
		}

		private async Task<bool> Notify2<T>(IContext context, T snapshot) where T: TaskSnapshot
		{
			if (context.Parent != null)
				context.Send(context.Parent, snapshot);
			await SendRealtime(snapshot);
			return true;
		}

		private async Task SendRealtime<T>(T snapshot) where T: TaskSnapshot
		{
			if (_definition is null) return;

			await (snapshot switch {
				TaskRemoved _ => _realtimeService.Task(_definition.Name, null),
				TaskSnapshot m => _realtimeService.Task(_definition.Name, m)
			});
		}

		private Task SendRealtime(LogEntry entry) =>
			_definition is null
				? Task.CompletedTask
				: _realtimeService.Log(_definition.Name, entry);

		private TaskDefinition NewDefinition(DefineTask request) =>
			new TaskDefinition {
				Name = request.Name,
				Executable = request.Executable,
				UseShell = request.UseShell,
				Arguments = request.Arguments,
				Directory = request.Directory,
				Environment = request.Environment.ToDictionary(),
				Tags = request.Tags.ToList(),
				ExpectedState = _definition?.ExpectedState ?? TaskState.Stopped
			};

		private TaskRemoved ToTaskRemoved(
			IRequest request, string id, TaskDefinition definition) =>
			new TaskRemoved(request, id, definition, _actualState, _startedTime);

		private TaskSnapshot ToTaskSnapshot(IRequest request) =>
			new TaskSnapshot(request, _definition!, _actualState, _startedTime);

		private TaskSnapshot ToTaskSnapshot() =>
			new TaskSnapshot(_definition!, _actualState, _startedTime);

		private static TaskNotFound ToTaskNotFound(TaskRef request) =>
			new TaskNotFound(request, request.Name);

		private static BadRequest ToBadRequest(SetTags request, string[] invalidTags) =>
			new BadRequest(
				request, request.Name, $"Tags '{invalidTags.Join(",")}' is invalid");

		private TaskCreated ToTaskCreated(IRequest request, string id) =>
			new TaskCreated(request, id, _definition!, _actualState, _startedTime);

		private TaskUpdated ToTaskUpdated(IRequest request, string id) =>
			new TaskUpdated(request, id, _definition!, _actualState, _startedTime);

		private static TaskDefinition NewDefinition(RemoveTask request) =>
			new TaskDefinition { Name = request.Name };

		protected override TaskDefinition GetSnapshot() =>
			_definition ?? throw new NullReferenceException("Definition has not been set");

		protected override void ApplySnapshot(TaskDefinition state) => _definition = state;
	}
}
