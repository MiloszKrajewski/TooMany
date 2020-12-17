using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttpRemoting.Data;
using K4os.RoutR.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Messages;

namespace TooMany.Cli.Handlers
{
	public class MonitorHandler: HostCommandHandler, ICommandHandler<MonitorCommand>
	{
		private readonly HubConnection _hubConnection;

		public MonitorHandler(
			ILoggerFactory loggerFactory,
			IHostInterface hostInterface,
			HubConnection hubConnection):
			base(loggerFactory, hostInterface)
		{
			_hubConnection = hubConnection;
		}

		public async Task Handle(MonitorCommand command, CancellationToken token)
		{
			_hubConnection.On(
				"Log",
				(string task, LogEntryResponse message) => OnLog(task, message));

			_hubConnection.On(
				"Task",
				(string task, TaskResponse message) => OnTask(task, message));

			await _hubConnection.StartAsync(token);

			(await Host.GetTasks()).ForEach(t => OnTask(t.Name, t));

			try
			{
				await Console.In.ReadLineAsync();
			}
			finally
			{
				await _hubConnection.StopAsync(token);
			}
		}

		private void OnLog(string task, LogEntryResponse message)
		{
			var level = message.Channel == LogChannel.StdErr
				? LogLevel.Warning
				: LogLevel.Debug;
			Log.Log(level, "{0}: {1}", task, message.Text);
		}

		private void OnTask(string task, TaskResponse message)
		{
			Log.LogInformation("{0}: {1}", task, message?.ActualState.ToString() ?? "REMOVED");
		}
	}
}
