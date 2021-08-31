using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TooMany.Actors.Worker.Processes
{
	public class ProcessSupervisor: IProcessSupervisor
	{
		private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);
		private static readonly TimeSpan LongInterval = TimeSpan.FromMilliseconds(333);
		private static readonly TimeSpan ShortInterval = TimeSpan.FromMilliseconds(33);

		protected ILogger Log { get; }

		private readonly IProcessKiller _killer;
		private readonly Action<LogEntry> _logAction;

		private readonly string _name;
		private readonly ProcessStartInfo _info;
		private Process? _proc;

		public ProcessSupervisor(
			ILoggerFactory loggerFactory,
			IProcessKiller killer,
			TaskDefinition definition,
			Action<LogEntry> logAction)
		{
			Log = loggerFactory.CreateLogger(GetType());
			_killer = killer;
			_logAction = logAction;
			_info = new ProcessStartInfo {
				FileName = BuildExecutable(definition),
				Arguments = BuildArguments(definition),
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = definition.Directory,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
			};
			UpdateEnvironment(_info.Environment, definition.Environment);
			_name = definition.Name;
		}

		private static string BuildExecutable(TaskDefinition definition) =>
			!definition.UseShell ? definition.Executable : "cmd";

		private static string BuildArguments(TaskDefinition definition) =>
			!definition.UseShell
				? definition.Arguments
				: $"/c {Quote(definition.Executable)} {definition.Arguments}";

		private static string Quote(string text, bool force = false) =>
			force || (text.Contains(' ') || text.Contains('\t'))
				? "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""
				: text;

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

		public async Task<Exception?> Start() =>
			// as this might be slow make sure
			// to create new thread for new process
			// rather than using one from pool
			await Task.Factory.StartNew(StartImpl, TaskCreationOptions.LongRunning);

		private Exception? StartImpl()
		{
			if (_proc is not null)
				throw new InvalidOperationException("Same process cannot be started again");

			try
			{
				Thread.Yield();
				
				var proc = new Process { StartInfo = _info };
				
				Log.LogInformation("Starting process: '{0}'", _name);
				proc.Start();
				LoopRead(proc.StandardOutput, false).Forget();
				LoopRead(proc.StandardError, true).Forget();
				
				_proc = proc;
				return null;
			}
			catch (Exception e)
			{
				Log.LogError("Process failed: '{0}'", _name);
				return e;
			}
		}

		private async Task LoopRead(TextReader reader, bool error)
		{
			try
			{
				while (true)
				{
					var line = await reader.ReadLineAsync();
					if (line is null) break;
					LogOutput(error, line);
				}
			}
			catch
			{
				Log.LogError("Reading output stream for '{0}' failed", _name);
			}
		}

		private void LogOutput(bool error, string? text)
		{
			if (text is null) return;

			_logAction(new LogEntry(error, Sanitize(text)));
		}

		private static string Sanitize(string text)
		{
			StringBuilder? sb = null;
			var head = 0;
			var length = text.Length;

			StringBuilder Builder() => sb ??= new StringBuilder(length);
			StringBuilder? Flush(int a, int b) => b > a ? Builder().Append(text, a, b - a) : sb;
			void Append(char c) => Builder().Append(c);

			for (var curr = 0; curr < length; curr++)
			{
				var c = text[curr];
				var space = c is '\t' or '\f';
				var flush = space || c is '\n' or '\r';
				if (!flush) continue;

				Flush(head, curr);
				head = curr + 1;
				if (space) Append(' ');
			}
			
			return sb == null ? text : Flush(head, text.Length)?.ToString() ?? string.Empty;
		}
		
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
			if (proc is null)
			{
				Log.LogWarning("Process already finished: '{0}'", _name);
				return 0;
			}

			if (!proc.HasExited)
				await Wait(_proc, LongInterval);

			Log.LogInformation("Process finished: '{0}'", _name);
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
