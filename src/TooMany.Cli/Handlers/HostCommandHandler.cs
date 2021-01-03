// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Net;
// using System.Threading.Tasks;
// using HttpRemoting.Data;
// using Microsoft.Extensions.Logging;
// using TooMany.Cli.Commands;
// using TooMany.Cli.UserInterface;
// using TooMany.Messages;
//
// namespace TooMany.Cli.Handlers
// {
// 	public abstract class HostCommandHandler
// 	{
// 		protected ILogger Log { get; set; }
// 		protected IHostInterface Host { get; set; }
//
// 		protected HostCommandHandler(
// 			ILoggerFactory loggerFactory,
// 			IHostInterface hostInterface)
// 		{
// 			Log = loggerFactory.CreateLogger(GetType());
// 			Host = hostInterface;
// 		}
//
// 		protected async Task<TaskResponse[]> GetTasks(IManyTasksOptions command)
// 		{
// 			var names = command.Names.ToArray();
// 			var tags = command.Tags;
//
// 			var many =
// 				names.Length != 1 ||
// 				names.Any(Wildcard.IsWildcard) ||
// 				!string.IsNullOrWhiteSpace(tags);
//
// 			if (many)
// 			{
// 				return FilterTasks(names, tags, await Host.GetTasks());
// 			}
// 			else
// 			{
// 				var name = names.Single();
//
// 				try
// 				{
// 					return new[] { await Host.GetTask(name) };
// 				}
// 				catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
// 				{
// 					Log.LogError("Tasks '{0}' not found", name);
// 					return Array.Empty<TaskResponse>();
// 				}
// 			}
// 		}
//
// 		protected async Task<TaskResponse[]> GetNamedTasks(string[] names)
// 		{
// 			if (names.Length == 0)
// 				return Array.Empty<TaskResponse>();
//
// 			var many = names.Length > 1;
//
// 			if (many)
// 			{
// 				var filter = names.ToHashSet(StringComparer.InvariantCultureIgnoreCase);
// 				return (await Host.GetTasks()).Where(t => filter.Contains(t.Name)).ToArray();
// 			}
//
// 			var name = names.Single();
//
// 			try
// 			{
// 				return new[] { await Host.GetTask(name) };
// 			}
// 			catch (HttpRemotingException e) when (e.StatusCode == HttpStatusCode.NotFound)
// 			{
// 				Log.LogError("Tasks '{0}' not found", name);
// 				return Array.Empty<TaskResponse>();
// 			}
// 		}
//
// 		private TaskResponse[] FilterTasks(
// 			string[] names, string? tags, TaskResponse[] tasks)
// 		{
// 			var tagsMatch = TagExpression.Matcher(tags);
// 			if (tagsMatch is null)
// 			{
// 				Log.LogWarning($"'{tags}' is not valid tag expression");
// 				return Array.Empty<TaskResponse>();
// 			}
//
// 			var filtered = new HashSet<TaskResponse>();
// 			foreach (var name in names)
// 			{
// 				var nameMatch = Wildcard.Matcher(name, true);
//
// 				foreach (var task in tasks)
// 				{
// 					if (filtered.Contains(task)) continue;
// 					if (!nameMatch(task.Name)) continue;
// 					if (!tagsMatch(task.Tags)) continue;
//
// 					filtered.Add(task);
// 				}
// 			}
//
// 			return filtered.OrderBy(t => t.Name).ToArray();
// 		}
// 	}
// }
