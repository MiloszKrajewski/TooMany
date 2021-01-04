using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Spectre.Console;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	public abstract class HostCommand<T>: AsyncCommand<T> where T: CommandSettings
	{
		protected IHostInterface Host { get; }

		protected HostCommand(IHostInterface host)
		{
			Host = host;
		}
		
		public async Task<TaskResponse[]> GetTasks(ManyTasksSettings settings)
		{
			var names = settings.Names.ToArray();
			var tags = settings.Tags;

			var many =
				names.Length != 1 ||
				names.Any(Wildcard.IsWildcard) ||
				!string.IsNullOrWhiteSpace(tags);

			if (many)
			{
				return FilterTasks(names, tags, await Host.GetTasks());
			}
			else
			{
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
		}
		
		protected async Task<TaskResponse[]> GetNamedTasks(string[] names)
		{
			if (names.Length == 0)
				return Array.Empty<TaskResponse>();

			var many = names.Length > 1;

			if (many)
			{
				var filter = names.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
				return (await Host.GetTasks()).Where(t => filter.Contains(t.Name)).ToArray();
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

		
		private static TaskResponse[] FilterTasks(
			string[] names, string? tags, TaskResponse[] tasks)
		{
			AnsiConsole.WriteLine();
			
			var tagsMatch = TagExpression.Matcher(tags);
			if (tagsMatch is null)
			{
				Presentation.Warn($"'{tags}' is not valid tag expression");
				return Array.Empty<TaskResponse>();
			}

			var filtered = new HashSet<TaskResponse>();
			foreach (var name in names)
			{
				var nameMatch = Wildcard.Matcher(name, true);

				foreach (var task in tasks)
				{
					if (filtered.Contains(task)) continue;
					if (!nameMatch(task.Name)) continue;
					if (!tagsMatch(task.Tags)) continue;

					filtered.Add(task);
				}
			}

			return filtered.OrderBy(t => t.Name).ToArray();
		}

		protected void ShowUnknownOptions(CommandContext context)
		{
			var parsed = context.Remaining.Parsed.Select(g => g.Key).ToArray();
			if (parsed.Length > 0) 
				Presentation.Warn($"Unknown options: {parsed.Join(", ")}");
		}

		protected void ShowIgnoredArguments(CommandContext context)
		{
			var remaining = context.Remaining.Raw.ToArray();
			if (remaining.Length > 0)
				Presentation.Warn($"Unknown arguments: {remaining.Join(", ")}");
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
	}
}
