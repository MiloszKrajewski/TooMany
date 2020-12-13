using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Handlers
{
	public class ApplyTagsHandler:
		HostCommandHandler,
		ICommandHandler<ApplyTagsCommand>
	{
		public ApplyTagsHandler(
			ILoggerFactory loggerFactory, IHostInterface hostInterface):
			base(loggerFactory, hostInterface) { }

		public async Task Handle(ApplyTagsCommand command, CancellationToken token)
		{
			var tasks = await GetTasks(command);
			if (tasks.Length <= 0) return;

			var responses = await Task.WhenAll(tasks.Select(t => UpdateTags(t, command)));

			Presentation.TaskInfo(responses);
		}

		private async Task<TaskResponse> UpdateTags(TaskResponse task, ApplyTagsCommand command)
		{
			var tags = new HashSet<string>(task.Tags, StringComparer.InvariantCultureIgnoreCase);
			tags.UnionWith(command.Set);
			tags.ExceptWith(command.Reset);

			if (tags.SetEquals(task.Tags))
				return task;

			return await Host.SetTags(task.Name, new TagsRequest { Tags = tags.ToList()! });
		}
	}
}
