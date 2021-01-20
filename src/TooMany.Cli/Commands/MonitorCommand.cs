using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	[Description("Monitor tasks")]
	public class MonitorCommand: HostCommand<MonitorCommand.Settings>
	{
		private static readonly object Lock = new object();

		public class Settings: CommandSettings
		{
			[CommandOption("-f|--filter <REGEX>")]
			[Description("Show only lines matching at least one given regular expression")]
			public string[] Filters { get; set; } = Array.Empty<string>();
		}

		protected HubConnection Hub { get; }

		public MonitorCommand(IHostInterface host, IRawArguments args, HubConnection hub):
			base(host, args)
		{
			Hub = hub;
		}

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var matcher = BuildLogFilter(settings.Filters);

			Hub.On(
				"Log",
				(string task, LogEntryResponse message) => OnLog(task, message, matcher));

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

		private static void OnLog(
			string task, LogEntryResponse message, Func<LogEntryResponse, bool> filter)
		{
			if (!filter(message)) return;

			lock (Lock) Presentation.LogEvent(task, message);
		}

		private static void OnTask(string task, TaskResponse message)
		{
			lock (Lock) Presentation.LogState(task, message);
		}
	}
}
