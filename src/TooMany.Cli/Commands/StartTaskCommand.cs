using System;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	public class StartTaskCommand: HostCommand<StartTaskCommand.Settings>
	{
		public class Settings: ManyTasksSettings
		{
			[CommandOption("-f|--force")]
			public bool Force { get; set; }
		}

		public StartTaskCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings);
			if (tasks.Length <= 0) return 0;

			var found = tasks.Select(t => t.Name).ToArray();

			await Task.WhenAll(tasks.Select(t => Host.StartTask(t.Name, settings.Force)));

			Presentation.TaskInfo(await GetNamedTasks(found));

			return 0;
		}
	}
}
