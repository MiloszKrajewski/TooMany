using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	[Description("Task details")]
	public class TaskSpecsCommand: HostCommand<TaskSpecsCommand.Settings>
	{
		public class Settings: ManyTasksSettings { }

		public TaskSpecsCommand(IHostInterface host, IRawArguments args): base(host, args) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");

			Presentation.TaskSpecs(tasks);

			return 0;
		}
	}
}
