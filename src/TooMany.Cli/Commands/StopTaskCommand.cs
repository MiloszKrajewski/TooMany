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

			var tasks = await GetTasks(settings);
			if (tasks.Length <= 0) return 0;

			var found = tasks.Select(t => t.Name).ToArray();
			
			await Task.WhenAll(tasks.Select(t => Host.StopTask(t.Name)));
			
			Presentation.TaskInfo(await GetNamedTasks(found));

			return 0;
		}
	}
}
