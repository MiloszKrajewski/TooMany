using System.Diagnostics;

namespace TooMany.Actors
{
	public interface IProcessKiller
	{
		void KillTree(Process process);
	}
}
