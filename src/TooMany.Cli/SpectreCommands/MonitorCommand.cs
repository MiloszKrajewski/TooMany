using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.SpectreCommands
{
	public class MonitorCommand: HostCommand<MonitorCommand.Settings>
	{
		public class Settings: CommandSettings { }
		
		protected HubConnection Hub { get; }

		public MonitorCommand(
			IHostInterface host,
			HubConnection hub): base(host)
		{
			Hub = hub;
		}

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			var _hubConnection = Hub;
			_hubConnection.On(
				"Log",
				(string task, LogEntryResponse message) => OnLog(task, message));

			_hubConnection.On(
				"Task",
				(string task, TaskResponse message) => OnTask(task, message));

			await _hubConnection.StartAsync();

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
				? ConsoleColor.Red
				: ConsoleColor.Cyan;
			Presentation.Log(task, message);
		}

		private void OnTask(string task, TaskResponse message)
		{
			Log.LogInformation("{0}: {1}", task, message?.ActualState.ToString() ?? "REMOVED");
		}

	}
}
