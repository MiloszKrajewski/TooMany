using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Handlers
{
	public class StartStopTaskHandler: 
		HostCommandHandler, 
		ICommandHandler<StartTaskCommand>,
		ICommandHandler<StopTaskCommand>
	{
		public StartStopTaskHandler(ILoggerFactory loggerFactory, IHostInterface hostInterface):
			base(loggerFactory, hostInterface) { }

		public async Task Handle(StartTaskCommand command, CancellationToken token)
		{
			var names = command.Names.ToArray();
			var tasks = await GetTasks(names);
			if (tasks.Length <= 0) return;

			var found = tasks.Select(t => t.Name).ToArray();
			
			await Task.WhenAll(tasks.Select(t => Host.StartTask(t.Name, command.Force)));
			Presentation.TaskInfo(await GetTasks(found));
		}
		
		public async Task Handle(StopTaskCommand command, CancellationToken token)
		{
			var names = command.Names.ToArray();
			var tasks = await GetTasks(names);
			if (tasks.Length <= 0) return;

			var found = tasks.Select(t => t.Name).ToArray();
			
			await Task.WhenAll(tasks.Select(t => Host.StopTask(t.Name)));
			Presentation.TaskInfo(await GetTasks(found));
		}

	}
}
