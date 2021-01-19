using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	[Description("Lists tasks")]
	public class ListTasksCommand: HostCommand<ListTasksCommand.Settings>
	{
		public class Settings: CommandSettings, IManyTasksSettings
		{
			[CommandArgument(0, "[TASK...]")]
			[Description("Names of tasks (wildcards are allowed, use '*' for all)")]
			public string[] Names { get; set; } = Array.Empty<string>();

			[CommandOption("--expression <EXPRESSION>")]
			[Description("Task filter expression (wildcards and logical operations are allowed, ie: \"~(a*|#b)&#c\")")]
			public string? Expression { get; set; }
		}

		public ListTasksCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings, true).WithSpinner("Getting task list...");

			Presentation.TaskInfo(tasks);

			return 0;
		}
	}
}
