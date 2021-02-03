using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Filters;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	[Description("Monitor tasks")]
	public class MonitorCommand: HostCommand<MonitorCommand.Settings>
	{
		private static readonly object Lock = new object();
		private readonly Dictionary<string, bool> _monitoredTasks = new();

		public class Settings: ManyTasksSettings
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

			var taskFilter = new TaskFilter(BuildTaskExpression(settings.Tasks));
			var logFilter = BuildLogFilter(settings.Filters);

			Hub.On(
				"Log",
				(string task, LogEntryResponse message) => OnLog(task, message, logFilter));

			Hub.On(
				"Task",
				(string task, TaskResponse message) => OnTask(task, message, taskFilter));

			await Hub.StartAsync();

			(await Host.GetTasks()).ForEach(t => OnTask(t.Name, t, taskFilter));

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

		private void OnLog(
			string task, LogEntryResponse message, Func<LogEntryResponse, bool> filter)
		{
			if (!filter(message)) return;

			lock (Lock)
			{
				_monitoredTasks.TryGetValue(task, out var active);
				if (active) Presentation.LogEvent(task, message);
			}
		}

		private void OnTask(string task, TaskResponse? message, TaskFilter filter)
		{
			lock (Lock)
			{
				var monitor = filter.IsMatch(message?.Name, message?.Tags);
				_monitoredTasks[task] = monitor;
				if (message is null) _monitoredTasks.Remove(task);
				Presentation.LogState(task, message);
			}
		}
	}
}
