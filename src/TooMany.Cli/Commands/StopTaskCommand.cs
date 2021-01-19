using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	[Description("Stop tasks")]
	public class StopTaskCommand: HostCommand<StopTaskCommand.Settings>
	{
		public class Settings: ManyTasksSettings { }

		public StopTaskCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");

			if (tasks.Length <= 0) return 0;

			var found = tasks.Select(t => t.Name).ToArray();

			await Task.WhenAll(found.Select(n => Host.StopTask(n))).WithSpinner("Stopping tasks...");

			var refreshed = await GetNamedTasks(found).WithSpinner("Refreshing task states...");

			Presentation.TaskInfo(refreshed);

			return 0;
		}
	}
}
