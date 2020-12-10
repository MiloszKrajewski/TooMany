using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using K4os.Json.Messages.Interfaces;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Persistence;
using TooMany.Actors.Messages;
using TooMany.Actors.Tools;
using TooMany.Messages;

namespace TooMany.Actors.Worker
{
	public class TaskRunnerActor: PersistentActor<TaskDefinition>, IActor
	{
		public const string ActorName = "TaskRunner";
		private const int LogHistorySize = 1000;

		private static readonly int MaxLogHistorySize =
			LogHistorySize + Math.Max(1, LogHistorySize >> 1);

		private TaskDefinition? _definition;

		private TaskState _actualState = TaskState.Stopped;
		private DateTime? _startedTime;
		private bool _rebootRequired;

		private Executor? _executor;
		private readonly Queue<LogEntry> _logHistory = new Queue<LogEntry>();

		private string TaskId => _definition?.Name ?? "<unknown>";

		public TaskRunnerActor(
			ILoggerFactory loggerFactory,
			IProvider persistenceProvider):
			base(loggerFactory, persistenceProvider) { }

		public Task ReceiveAsync(IContext context) =>
			context.Message switch {
				Started _ => OnStarted(context),
				Stopping _ => OnStopping(context),
				// ----
				DefineTask m => OnDefineTask(context, m),
				RemoveTask m => OnRemoveTask(context, m),
				// ----
				GetTask m => OnGetTask(context, m),
				StartTask m => OnStartTask(context, m),
				StopTask m => OnStopTask(context, m),
				// ----
				LogEntry e => OnLogAdded(e),
				GetLog m => OnGetLog(context, m),
				SyncState _ => OnSyncState(context),
				_ => Task.CompletedTask
			};

		private Task OnLogAdded(LogEntry entry)
		{
			_logHistory.Enqueue(entry);
			TrimLog();
			return Task.CompletedTask;
		}

		private void TrimLog()
		{
			if (_logHistory.Count <= MaxLogHistorySize)
				return;

			while (_logHistory.Count > LogHistorySize)
				_logHistory.Dequeue();
		}

		private Task OnGetLog(IContext context, GetLog request)
		{
			var messages = _logHistory.ToArray();
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

		private Task OnStopTask(IContext context, StopTask request) =>
			UpdateState(context, request, TaskState.Stopped);

		private Task OnSyncState(IContext context)
		{
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

		public Task StartProcess(IContext context, TaskDefinition definition)
		{
			Log.LogInformation($"Starting '{TaskId}'");
			var executor = new Executor(definition, context.System, context.Self!);
			var exception = executor.Start();
			return exception is null
				? OnProcessStarted(context, executor)
				: OnProcessFailed(context, exception);
		}

		private async Task StopProcess(IContext context)
		{
			if (_executor is null)
				return;

			Log.LogInformation($"Stopping '{TaskId}'");
			if (await _executor.Stop()) return;

			Log.LogWarning($"Killing '{TaskId}'");
			if (await _executor.Kill()) return;

			Log.LogError($"Failed to stop '{TaskId}', scheduling retry...");
			ScheduleSync(context);
		}

		private Task OnProcessStarted(IContext context, Executor executor)
		{
			Log.LogInformation($"Task '{TaskId}' started");
			context.ReenterAfter(executor.Wait(), t => OnProcessStopped(context, t.Result));
			_actualState = TaskState.Started;
			_startedTime = DateTime.UtcNow;
			_executor = executor;
			return Task.CompletedTask;
		}

		private Task OnProcessFailed(IContext context, Exception exception)
		{
			Log.LogError(exception, $"Task '{TaskId}' failed to start");
			ScheduleSync(context);
			_actualState = TaskState.Failed;
			_startedTime = null;
			_rebootRequired = false;
			_executor = null;
			return Task.CompletedTask;
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
			_executor = null;
			return Task.CompletedTask;
		}

		private static void ScheduleSync(IContext context, bool immediate = false)
		{
			var delay = immediate ? 0 : 1;
			context.SendLater(context.Self!, TimeSpan.FromSeconds(delay), () => new SyncState());
		}

		private async Task OnStarted(IContext context)
		{
			await RestoreState(context);
			ScheduleSync(context, true);
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
			await (
				created
					? context.Return(ToTaskCreated(request, id), true)
					: context.Return(ToTaskUpdated(request, id), true)
			);

			await StopProcess(context);
			ScheduleSync(context, true);
		}

		private static TaskNotFound ToTaskNotFound(GetTask request) =>
			new TaskNotFound(request, request.Name);

		private TaskCreated ToTaskCreated(IRequest request, string id) =>
			new TaskCreated(request, id, _definition!, _actualState, _startedTime);

		private TaskUpdated ToTaskUpdated(IRequest request, string id) =>
			new TaskUpdated(request, id, _definition!, _actualState, _startedTime);

		private TaskRemoved ToTaskRemoved(IRequest request, string id, TaskDefinition definition) =>
			new TaskRemoved(request, id, definition, _actualState, _startedTime);

		private TaskSnapshot ToTaskSnapshot(IRequest request) =>
			new TaskSnapshot(request, _definition!, _actualState, _startedTime);

		private async Task UpdateState(IContext context, IRequest request, TaskState state)
		{
			if (_definition is null) return;

			_definition.ExpectedState = state;
			await Persist();
			ScheduleSync(context, true);

			context.Respond(ToTaskSnapshot(request));
		}

		private Task OnRemoveTask(IContext context, RemoveTask request)
		{
			var id = context.Self!.Id;
			context.Stop(context.Self!);
			var definition = _definition ?? NewDefinition(request);
			context.Return(ToTaskRemoved(request, id, definition), true);
			return Task.CompletedTask;
		}

		private TaskDefinition NewDefinition(DefineTask request) =>
			new TaskDefinition {
				Name = request.Name,
				Executable = request.Executable,
				Arguments = request.Arguments,
				Directory = request.Directory,
				Environment = request.Environment.ToDictionary(),
				ExpectedState = _definition?.ExpectedState ?? TaskState.Stopped
			};

		private static TaskDefinition NewDefinition(RemoveTask request) =>
			new TaskDefinition { Name = request.Name };

		protected override TaskDefinition GetSnapshot() =>
			_definition ?? throw new NullReferenceException("Definition has not been set");

		protected override void ApplySnapshot(TaskDefinition state) => _definition = state;
	}
}
