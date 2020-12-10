using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Persistence;
using Proto.Persistence.SnapshotStrategies;
using NullLoggerFactory = Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory;

namespace TooMany.Actors.Tools
{
	public abstract class PersistentActor<TState>
	{
		protected ILogger Log { get; }

		private readonly IProvider _provider;
		private readonly bool _eventSourcing;
		
		private Persistence? _persister;
		
		protected PersistentActor(
			ILoggerFactory? loggerFactory,
			IProvider provider, bool eventSourcing = false)
		{
			Log = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger(GetType());
			_provider = provider;
			_eventSourcing = eventSourcing;
		}

		protected Task RestoreState(IInfoContext context) =>
			RestoreState(context.Self!.Id);

		protected virtual async Task RestoreState(string actorId)
		{
			_persister = _eventSourcing
				? CreateEventSourcingPersister(actorId)
				: CreateSnapshottingPersister(actorId);
			await _persister.RecoverStateAsync();
		}

		private Persistence CreateSnapshottingPersister(string actorId) =>
			Persistence.WithSnapshotting(_provider, actorId, ApplySnapshotProxy);

		private Persistence CreateEventSourcingPersister(string actorId) =>
			Persistence.WithEventSourcingAndSnapshotting(
				_provider, _provider, actorId,
				ApplyEventProxy, ApplySnapshotProxy, new IntervalStrategy(16), GetSnapshotProxy);

		protected virtual Task Persist() =>
			Persister.PersistSnapshotAsync(GetSnapshotProxy());

		protected virtual Task Persist(object @event) =>
			Persister.PersistEventAsync(@event);
		
		private object GetSnapshotProxy() => GetSnapshot()!;

		private void ApplySnapshotProxy(Snapshot snapshot)
		{
			var state = snapshot.State;
			var updated = state is TState latest ? latest : UpgradeSnapshot(state);
			ApplySnapshot(updated);
		}

		private void ApplyEventProxy(Event @event) => ApplyEvent(@event.Data);
		
		protected abstract TState GetSnapshot();

		protected abstract void ApplySnapshot(TState state);
		
		protected virtual TState UpgradeSnapshot(object state) =>
			throw NotImplemented(nameof(UpgradeSnapshot), state.GetType().FullName ?? "<unknown>");

		protected virtual void ApplyEvent(object @event) =>
			throw NotImplemented(nameof(ApplyEvent));

		private Persistence Persister =>
			_persister ?? throw new NullReferenceException("Persistence layer is not initialized");

		private static Exception NotImplemented(string methodName) =>
			new NotImplementedException($"{methodName} is not implemented");

		private static Exception NotImplemented(string methodName, string details) =>
			new NotImplementedException($"{methodName} is not implemented for {details}");
	}
}
