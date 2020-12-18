using System.Diagnostics;

namespace TooMany.Actors.Worker.Processes
{
	public interface IProcessKiller
	{
		void KillTree(Process process);
	}
}
