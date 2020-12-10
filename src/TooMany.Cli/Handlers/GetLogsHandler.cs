using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttpRemoting.Data;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Messages;

namespace TooMany.Cli.Handlers
{
	public class GetLogsHandler: HostCommandHandler, ICommandHandler<GetLogsCommand>
	{
		public GetLogsHandler(
			ILoggerFactory loggerFactory,
			IHostInterface hostInterface):
			base(loggerFactory, hostInterface) { }

		public async Task Handle(GetLogsCommand command, CancellationToken token)
		{
			var logs = await GetTaskLog(command.Name);
			var sequence = command.Tail.HasValue ? logs.TakeLast(command.Tail.Value) : logs;
			sequence.ForEach(Print);
		}

		private async Task<LogEntryResponse[]> GetTaskLog(string name)
		{
			try
			{
				return await Host.GetTaskLog(name);
			}
			catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				Log.LogError("Task '{0}' could not be found", name);
				return Array.Empty<LogEntryResponse>();
			}
		}

		private void Print(LogEntryResponse entry)
		{
			var level = entry.Channel switch {
				LogChannel.StdOut => LogLevel.Information,
				LogChannel.StdErr => LogLevel.Error,
				_ => LogLevel.Trace
			};
			Log.Log(level, entry.Text);
		}
	}
}
