using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Microsoft.Extensions.Logging;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Handlers
{
	public abstract class HostCommandHandler
	{
		protected ILogger Log { get; set; }
		protected IHostInterface Host { get; set; }

		protected HostCommandHandler(
			ILoggerFactory loggerFactory, 
			IHostInterface hostInterface)
		{
			Log = loggerFactory.CreateLogger(GetType());
			Host = hostInterface;
		}

		protected async Task<TaskResponse[]> GetTasks(string[] names)
		{
			var many = names.Length != 1 || names.Any(Wildcard.IsWildcard);

			if (many)
			{
				return ValidateTasks(names, await Host.GetTasks());
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
					Log.LogError("Tasks '{0}' not found", name);
					return Array.Empty<TaskResponse>();
				}
			}
		}
		
		private TaskResponse[] ValidateTasks(string[] names, TaskResponse[] tasks)
		{
			var filtered = new HashSet<TaskResponse>();
			foreach (var name in names)
			{
				var matcher = Wildcard.Matcher(name, true);
				var found = 0;

				foreach (var task in tasks)
				{
					if (!matcher(task.Name)) continue;

					filtered.Add(task);
					found++;
				}

				if (found <= 0) Log.LogWarning("No tasks found for '{0}'", name);
			}

			return filtered.OrderBy(t => t.Name).ToArray();
		}

	}
}
