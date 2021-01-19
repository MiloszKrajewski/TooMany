using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using K4os.Shared;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Commands
{
	[Description("Remove tasks")]
	public class RemoveTaskCommand: HostCommand<RemoveTaskCommand.Settings>
	{
		public class Settings: ManyTasksSettings
		{
			[CommandOption("-f|--force")]
			[Description("Confirms removal when using wildcards")]
			public bool Force { get; set; }
		}

		public RemoveTaskCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var names = settings.Tasks;
			if (names.Any(IsExpression) && !settings.Force)
			{
				Presentation.Error(
					"When removing tasks using wildcard '--force' switch is required");
				return 0;
			}

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");
			if (tasks.Length <= 0) return 0;

			Presentation.TaskInfo(tasks);

			await Task.WhenAll(tasks.Select(t => Host.RemoveTask(t.Name)))
				.WithSpinner("Removing tasks...");

			return 0;
		}
	}
}
