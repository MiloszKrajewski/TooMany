using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.UserInterface;

namespace TooMany.Cli.Handlers
{
	public class TaskInfoHandler:
		HostCommandHandler,
		ICommandHandler<ListTaskCommand>,
		ICommandHandler<TaskInfoCommand>
	{
		public TaskInfoHandler(
			ILoggerFactory loggerFactory,
			IHostInterface hostInterface):
			base(loggerFactory, hostInterface) { }

		public async Task Handle(ListTaskCommand command, CancellationToken token)
		{
			var names = command.Names.ToArray();
			if (names.Length <= 0) names = new[] { "*" };
			var tasks = await GetTasks(command);

			if (tasks.Length <= 0) return;

			Presentation.TaskInfo(tasks);
		}

		public async Task Handle(TaskInfoCommand command, CancellationToken token)
		{
			var tasks = await GetTasks(command);

			if (tasks.Length <= 0) return;

			Presentation.TaskDetails(tasks);
		}
	}
}
