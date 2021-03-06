using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	[Description("Restart tasks")]
	public class RestartTaskCommand: HostCommand<RestartTaskCommand.Settings>
	{
		public class Settings: ManyTasksSettings { }

		public RestartTaskCommand(IHostInterface host, IRawArguments args): base(host, args) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");
			if (tasks.Length <= 0) return 0;

			var found = tasks
				.Where(t => t.ExpectedState == TaskState.Started)
				.Select(t => t.Name)
				.ToArray();

			await Task.WhenAll(found.Select(n => Host.StartTask(n, true)))
				.WithSpinner("Restarting task...");

			var refreshed = await GetNamedTasks(found).WithSpinner("Refreshing task states...");
			
			Presentation.TaskInfo(refreshed);

			return 0;
		}
	}
}
