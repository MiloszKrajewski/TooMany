using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	[Description("Apply tags (set and clear) to tasks")]
	public class ApplyTagsCommand: HostCommand<ApplyTagsCommand.Settings>
	{
		public class Settings: ManyTasksSettings
		{
			[CommandOption("-s|--set <TAGS>")]
			[Description("Tags to set")]
			public string[] Set { get; set; } = Array.Empty<string>();

			[CommandOption("-c|--clear <TAGS>")]
			[Description("Tags to clear")]
			public string[] Clear { get; set; } = Array.Empty<string>();
		}

		public ApplyTagsCommand(IHostInterface host, IRawArguments args): base(host, args) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);

			var tasks = await GetTasks(settings).WithSpinner("Getting task list...");

			if (tasks.Length <= 0) return 0;

			var responses = await Task.WhenAll(tasks.Select(t => UpdateTags(t, settings)))
				.WithSpinner("Updating tags...");

			Presentation.TaskInfo(responses);

			return 0;
		}

		private async Task<TaskResponse> UpdateTags(TaskResponse task, Settings settings)
		{
			var tags = new HashSet<string>(task.Tags, StringComparer.InvariantCultureIgnoreCase);
			tags.UnionWith(ExpandTags(settings.Set));
			tags.ExceptWith(ExpandTags(settings.Clear));

			if (tags.SetEquals(task.Tags))
				return task;

			return await Host.SetTags(task.Name, new TagsRequest { Tags = tags.ToList() });
		}
	}
}
