using System;

namespace TooMany.Actors.Worker.Processes
{
	public interface IProcessFactory
	{
		IProcessSupervisor Create(TaskDefinition definition, Action<LogEntry> logAction);
	}
}
