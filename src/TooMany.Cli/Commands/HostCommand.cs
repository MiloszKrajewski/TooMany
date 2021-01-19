using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	public abstract class HostCommand<T>: AsyncCommand<T> where T: CommandSettings
	{
		protected IHostInterface Host { get; }

		protected HostCommand(IHostInterface host) { Host = host; }

		public bool IsExpression(string task)
		{
			var wildcards = new[] { '*', '?', '|', '&', '~', '#' };
			return task.Any(c => wildcards.Contains(c));
		}

		public async Task<TaskResponse[]> GetTasks(
			IManyTasksSettings settings, bool listAllIfNoNames = false)
		{
			var tasks = settings.Tasks;

			return (tasks.Length switch {
				0 when listAllIfNoNames => await Host.GetTasks(),
				0 => Array.Empty<TaskResponse>(),
				1 => await Host.GetTasks(tasks.First()),
				_ => await Host.GetTasks(tasks.Select(n => $"({n})").Join("|"))
			}).OrderBy(t => t.Name).ToArray();
		}

		protected async Task<TaskResponse[]> GetNamedTasks(string[] names)
		{
			if (names.Length == 0)
				return Array.Empty<TaskResponse>();

			var many = names.Length > 1;

			if (many)
			{
				var filter = names.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
				return (await Host.GetTasks())
					.Where(t => filter.Contains(t.Name))
					.OrderBy(t => t.Name).ToArray();
			}

			var name = names.Single();

			try
			{
				return new[] { await Host.GetTask(name) };
			}
			catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
			{
				Presentation.Error($"Tasks '{name}' not found");
				return Array.Empty<TaskResponse>();
			}
		}

		protected void ShowUnknownOptions(CommandContext context)
		{
			// NOTE: it is not handled as expected by Spectre
			// var parsed = context.Remaining.Parsed.Select(g => g.Key).ToArray();
			// if (parsed.Length > 0) 
			// 	Presentation.Warn($"Unknown options: {parsed.Join(", ")}");
		}

		protected void ShowIgnoredArguments(CommandContext context)
		{
			// NOTE: it is not handled as expected by Spectre
			// var remaining = context.Remaining.Raw.ToArray();
			// if (remaining.Length > 0)
			// 	Presentation.Warn($"Unknown arguments: {remaining.Join(", ")}");
		}

		protected static Func<LogEntryResponse, bool> BuildLogFilter(
			IReadOnlyCollection<string> filters)
		{
			static bool NotEmpty(LogEntryResponse e) => !string.IsNullOrEmpty(e.Text);
			if (filters.Count <= 0) return NotEmpty;

			var regex = filters.Select(f => new Regex(f, RegexOptions.Singleline)).ToArray();
			bool MatchesAny(LogEntryResponse e) => regex.Any(r => r.IsMatch(e.Text ?? ""));

			return e => NotEmpty(e) && MatchesAny(e);
		}

		protected static string[] ExpandTags(IEnumerable<string> tags) =>
			TagExpression.ExpandTags(tags);
	}
}
