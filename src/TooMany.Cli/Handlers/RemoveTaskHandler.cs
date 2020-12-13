using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Handlers
{
	public class RemoveTaskHandler: 
		HostCommandHandler, 
		ICommandHandler<RemoveTaskCommand>
	{
		public RemoveTaskHandler(ILoggerFactory loggerFactory, IHostInterface hostInterface):
			base(loggerFactory, hostInterface) { }

		public async Task Handle(RemoveTaskCommand command, CancellationToken token)
		{
			var names = command.Names.ToArray();
			if (names.Any(Wildcard.IsWildcard) && !command.Force)
			{
				Log.LogError("When removing tasks using wildcard '--force' switch is required");
				return;
			}
				
			var tasks = await GetTasks(command);
			if (tasks.Length <= 0) return;

			Presentation.TaskInfo(tasks);

			await Task.WhenAll(tasks.Select(t => Host.RemoveTask(t.Name)));
		}
	}
}
