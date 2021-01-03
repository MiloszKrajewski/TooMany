using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.SpectreCommands
{
	[Description("Lists tasks matching given patterns")]
	public class ListTasksCommand: HostCommand<ListTasksCommand.Settings>
	{
		public class Settings: ManyTasksSettings
		{
			[CommandOption("-d|--details")]
			[Description("Shows a little bit more details about tasks")]
			public bool Details { get; set; }
		}

		public ListTasksCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings);

			if (settings.Details)
			{
				Presentation.TaskDetails(tasks);
			}
			else
			{
				Presentation.TaskInfo(tasks);
			}

			return 0;
		}
	}
}
