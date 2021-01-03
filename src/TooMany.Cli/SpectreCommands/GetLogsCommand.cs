using System;
using System.Reactive.Linq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.SpectreCommands
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
				.SelectMany(t => GetTaskLog(t.Name))
				.SelectMany(x => x)
				.ToArray();
			entries
				.OrderBy(e => e.Timestamp)
				.ForEach(Print);
			return 0;
		}

		private async Task<LogEntryResponse[]> GetTaskLog(string name)
		{
			try
			{
				return await Host.GetTaskLog(name);
			}
			catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				Presentation.Error($"Task '{name}' could not be found");
				return Array.Empty<LogEntryResponse>();
			}
		}

		private static void Print(LogEntryResponse entry)
		{
			var color = entry.Channel switch {
				LogChannel.StdOut => ConsoleColor.White,
				LogChannel.StdErr => ConsoleColor.Red,
				_ => ConsoleColor.Gray
			};
			Presentation.WriteLine(color, entry.Text ?? string.Empty);
		}
	}
}
