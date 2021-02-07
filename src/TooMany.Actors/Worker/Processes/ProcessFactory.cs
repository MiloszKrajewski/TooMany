using System;
using Microsoft.Extensions.Logging;

namespace TooMany.Actors.Worker.Processes
{
	public class ProcessFactory: IProcessFactory
	{
		private readonly IProcessKiller _killer;
		private readonly ILoggerFactory _loggerFactory;

		public ProcessFactory(ILoggerFactory loggerFactory, IProcessKiller killer)
		{
			_killer = killer;
			_loggerFactory = loggerFactory;
		}

		public IProcessSupervisor Create(TaskDefinition definition, Action<LogEntry>? logAction) =>
			new ProcessSupervisor(_loggerFactory, _killer, definition, logAction ?? (_ => { }));
	}
}
