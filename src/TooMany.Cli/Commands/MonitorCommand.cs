using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
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
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			Hub.On(
				"Log",
				(string task, LogEntryResponse message) => OnLog(task, message));

			Hub.On(
				"Task",
				(string task, TaskResponse message) => OnTask(task, message));

			await Hub.StartAsync();

			(await Host.GetTasks()).ForEach(t => OnTask(t.Name, t));

			try
			{
				await Console.In.ReadLineAsync();
			}
			finally
			{
				await Hub.StopAsync();
			}

			return 0;
		}

		private static void OnLog(string task, LogEntryResponse message)
		{
			Presentation.LogEvent(task, message);
		}

		private static void OnTask(string task, TaskResponse message)
		{
			Presentation.LogState(task, message);
		}
	}
}
