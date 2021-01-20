using System;
using System.Reactive.Linq;
using System.Linq;
using System.Threading.Tasks;
using K4os.Json.Messages.Interfaces;
using K4os.Text.BaseX;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Persistence;
using TooMany.Actors.Filter;
using TooMany.Actors.Messages;
using TooMany.Actors.Tools;
using TooMany.Actors.Worker;

namespace TooMany.Actors.Catalog
{
	public class TaskCatalogActor: PersistentActor<TaskCatalog>, IActor
	{
		public const string ActorName = "TaskCatalog";

		private TaskCatalog _catalog = new TaskCatalog();

		private readonly Props _workerProps;

		public TaskCatalogActor(
			ILoggerFactory loggerFactory,
			IProvider persistenceProvider,
			ITypedProps<TaskRunnerActor> workerProps):
			base(loggerFactory, persistenceProvider, true)
		{
			_workerProps = workerProps.Props;
		}

		public Task ReceiveAsync(IContext context) =>
			context.Message switch {
				Started _ => OnStarted(context),
				//
				GetTask m => Forward(context, m),
				StartTask m => Forward(context, m),
				StopTask m => Forward(context, m),
				RemoveTask m => Forward(context, m),
				SetTags m => Forward(context, m),
				GetLog m => Forward(context, m),
				//
				DefineTask m => OnDefineTask(context, m),
				GetTasks m => OnGetTasks(context, m),
				//
				TaskCreated m => Persist(m),
				TaskRemoved m => Persist(m),
				TaskSnapshot _ => Task.CompletedTask, // explicitly ignore
				// 
				_ => Task.CompletedTask
			};

		private PID? Find(IContext context, string taskName) =>
			_catalog.Tasks.TryGetValue(taskName, out var entry)
				? context.Children.FirstOrDefault(c => c.Id == entry.Id)
				: null;

		private PID? Find(IContext context, TaskRef taskRef) =>
			Find(context, taskRef.Name);

		private PID Spawn(ISpawnerContext context, string? id = null) =>
			context.SpawnNamed(
				_workerProps, id ?? $"{TaskRunnerActor.ActorName}/{ShortGuid.NewGuid()}");

		private Task Forward<T>(IContext context, T request) where T: TaskRef
		{
			var taskName = request.Name;
			var worker = Find(context, taskName);
			if (worker is null) return context.Return(new TaskNotFound(request, taskName));

			context.Request(worker, request, context.Sender);
			return Task.CompletedTask;
		}

		private async Task OnStarted(IContext context)
		{
			await RestoreState(context);
			foreach (var task in _catalog.Tasks.Values)
				Spawn(context, context.ToRelativeId(task.Id));
		}

		private Task OnDefineTask(IContext context, DefineTask message)
		{
			var worker = Find(context, message) ?? Spawn(context);
			context.Forward(worker, message);
			return Task.CompletedTask;
		}

		private async Task OnGetTasks(IContext context, GetTasks request)
		{
			var filter = TaskFilter.TryCreate(request.Filter);
			if (filter is null)
			{
				context.Respond(new InvalidFilter(request, request.Filter));
				return;
			}

			var tasks = await _catalog.Tasks.Values
				.ToObservable()
				.SelectMany(t => GetTaskDefinition(context, t.Name))
				.Where(filter.IsMatch)
				.ToArray();

			context.Respond(new ManyTasksSnapshot(request, tasks.NoNulls()));
		}

		private async Task<TaskSnapshot?> GetTaskDefinition(IContext context, string name)
		{
			var pid = Find(context, name);
			if (pid is null) return null;

			var response = await context.RequestAsync<IMessage>(pid, new GetTask());
			return response as TaskSnapshot;
		}

		protected override TaskCatalog GetSnapshot() =>
			_catalog;

		protected override void ApplySnapshot(TaskCatalog state) =>
			_catalog = new TaskCatalog(state);

		protected override void ApplyEvent(object @event)
		{
			switch (@event)
			{
				case TaskCreated e:
					Apply(e);
					break;
				case TaskRemoved e:
					Apply(e);
					break;
			}
		}

		private void Apply(TaskCreated e) =>
			_catalog.Tasks[e.Name] = new TaskCatalogEntry(e);

		private void Apply(TaskRemoved e) =>
			_catalog.Tasks.Remove(e.Name);
	}
}
