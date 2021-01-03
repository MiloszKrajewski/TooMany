using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	public class GetLogsCommand: HostCommand<GetLogsCommand.Settings>
	{
		public GetLogsCommand(IHostInterface host): base(host) { }

		public class Settings: ManyTasksSettings { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings);
			var entries = await tasks
				.ToObservable()
				.SelectMany(GetTaskLog).SelectMany(x => x)
				.ToArray();
			entries
				.OrderBy(e => e.Log.Timestamp)
				.ForEach(e => Print(e.Task, e.Log));
			return 0;
		}

		private async Task<(TaskResponse Task, LogEntryResponse Log)[]> GetTaskLog(
			TaskResponse task)
		{
			try
			{
				var entries = await Host.GetTaskLog(task.Name);
				return entries.Select(e => (task, e)).ToArray();
			}
			catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				Presentation.Error($"Task '{task.Name}' could not be found");
				return Array.Empty<(TaskResponse, LogEntryResponse)>();
			}
		}

		private static void Print(TaskResponse task, LogEntryResponse entry)
		{
			Presentation.LogEvent(task.Name, entry);
		}
	}
}
