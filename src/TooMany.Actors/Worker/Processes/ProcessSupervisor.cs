using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TooMany.Actors.Worker.Processes
{
	public class ProcessSupervisor: IProcessSupervisor
	{
		private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);
		private static readonly TimeSpan LongInterval = TimeSpan.FromMilliseconds(333);
		private static readonly TimeSpan ShortInterval = TimeSpan.FromMilliseconds(33);

		private readonly IProcessKiller _killer;
		private readonly Action<LogEntry> _logAction;

		private readonly ProcessStartInfo _info;
		private Process? _proc;

		public ProcessSupervisor(
			IProcessKiller killer,
			TaskDefinition definition,
			Action<LogEntry> logAction)
		{
			_killer = killer;
			_logAction = logAction;
			_info = new ProcessStartInfo {
				FileName = definition.Executable,
				Arguments = definition.Arguments,
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = definition.Directory,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
			};
			UpdateEnvironment(_info.Environment, definition.Environment);
		}

		private static void UpdateEnvironment(
			IDictionary<string, string> current, IDictionary<string, string?> expected)
		{
			foreach (var (key, value) in expected)
			{
				if (value is null)
					current.Remove(key);
				else
					current[key] = value;
			}
		}

		public Exception? Start()
		{
			if (_proc is { })
				throw new InvalidOperationException("Same process cannot be started again");

			try
			{
				var proc = new Process { StartInfo = _info };
				proc.ErrorDataReceived += (_, args) => OnErrorReceived(args.Data);
				proc.OutputDataReceived += (_, args) => OnOutputReceived(args.Data);
				proc.Start();
				proc.BeginOutputReadLine();
				proc.BeginErrorReadLine();
				_proc = proc;
				return null;
			}
			catch (Exception e)
			{
				return e;
			}
		}

		private void Log(bool error, string text) =>
			_logAction(new LogEntry(error, text));

		private void OnOutputReceived(string text) => Log(false, text);

		private void OnErrorReceived(string text) => Log(true, text);

		public async Task<bool> Stop()
		{
			var proc = _proc;

			if (proc is null || proc.HasExited) return true;

			var sent = proc.CloseMainWindow();
			if (!sent) return false;

			return await Wait(proc, WaitTimeout, ShortInterval);
		}

		public async Task<bool> Kill()
		{
			var proc = _proc;

			if (proc is null || proc.HasExited) return true;

			KillTree(proc);

			return await Wait(proc, WaitTimeout, ShortInterval);
		}

		private void KillTree(Process proc) => _killer.KillTree(proc);

		public async Task<int> Wait()
		{
			var proc = _proc;
			if (proc is null) return 0;
			if (proc.HasExited) return proc.ExitCode;

			await Wait(_proc, LongInterval);
			return proc.ExitCode;
		}

		private static Task<bool> Wait(Process? proc, TimeSpan limit, TimeSpan interval) =>
			Wait(proc, DateTime.UtcNow.Add(limit), interval);

		private static Task<bool> Wait(Process? proc, TimeSpan interval) =>
			Wait(proc, DateTime.MaxValue, interval);

		private static async Task<bool> Wait(Process? proc, DateTime limit, TimeSpan interval)
		{
			if (proc is null || proc.HasExited) return true;

			while (true)
			{
				if (proc.HasExited) return true;
				if (DateTime.UtcNow > limit) return false;

				await Task.Delay(interval);
			}
		}
	}
}
