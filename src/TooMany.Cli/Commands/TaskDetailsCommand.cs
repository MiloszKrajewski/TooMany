using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	[Description("Task details")]
	public class TaskDetailsCommand: HostCommand<TaskDetailsCommand.Settings>
	{
		public class Settings: ManyTasksSettings { }

		public TaskDetailsCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");

			Presentation.TaskDetails(tasks);

			return 0;
		}
	}
}
