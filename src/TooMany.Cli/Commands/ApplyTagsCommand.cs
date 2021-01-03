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
	public class ApplyTagsCommand: HostCommand<ApplyTagsCommand.Settings>
	{
		public class Settings: ManyTasksSettings
		{
			[CommandOption("-s|--set <FLAG>")]
			[Description("Flag to set")]
			public string[] Set { get; set; } = Array.Empty<string>();

			[CommandOption("-c|--clear <FLAG>")]
			[Description("Flags to clear")]
			public string[] Clear { get; set; } = Array.Empty<string>();
		}

		public ApplyTagsCommand(IHostInterface host): base(host) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			ShowIgnoredArguments(context);
			
			var tasks = await GetTasks(settings);
			if (tasks.Length <= 0) return 0;

			var responses = await Task.WhenAll(tasks.Select(t => UpdateTags(t, settings)));

			Presentation.TaskInfo(responses);

			return 0;
		}
		
		private async Task<TaskResponse> UpdateTags(TaskResponse task, Settings settings)
		{
			var tags = new HashSet<string>(task.Tags, StringComparer.InvariantCultureIgnoreCase);
			tags.UnionWith(settings.Set);
			tags.ExceptWith(settings.Clear);

			if (tags.SetEquals(task.Tags))
				return task;

			return await Host.SetTags(task.Name, new TagsRequest { Tags = tags.ToList() });
		}
	}
}
