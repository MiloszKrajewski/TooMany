using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using TooMany.Actors.Worker.Processes;

namespace TooMany.Host
{
	public class ProcessKiller: IProcessKiller
	{
		public void KillTree(Process process)
		{
			Children(process).ForEach(KillTree);
			process.Kill();
		}

		private static IEnumerable<int> ChildrenPidSequence(int pid)
		{
			var query = $"select * from Win32_Process where ParentProcessID = {pid}";
			using var searcher = new ManagementObjectSearcher(query);
			var collection = searcher.Get().Cast<ManagementObject>();
			static int TryGetProcessId(ManagementObject o) => Convert.ToInt32(o["ProcessId"]);
			return collection.Select(TryGetProcessId).ToArray();
		}

		private static IEnumerable<Process> Children(Process process) =>
			process.HasExited
				? Array.Empty<Process>()
				: ChildrenPidSequence(process.Id).Select(Process.GetProcessById);
	}
}
