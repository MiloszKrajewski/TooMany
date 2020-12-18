using System;

namespace TooMany.Actors.Worker.Processes
{
	public class ProcessFactory: IProcessFactory
	{
		private readonly IProcessKiller _killer;

		public ProcessFactory(IProcessKiller killer) => _killer = killer;

		public IProcessSupervisor Create(TaskDefinition definition, Action<LogEntry>? logAction) =>
			new ProcessSupervisor(_killer, definition, logAction ?? (_ => { }));
	}
}
