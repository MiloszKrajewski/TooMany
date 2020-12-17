using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TooMany.Actors;
using TooMany.Actors.Messages;
using TooMany.Actors.Worker;
using TooMany.WebServer.Messages;

namespace TooMany.WebServer
{
	public class RealtimeService: IRealtimeService
	{
		private readonly IHubContext<MonitorHub> _hub;

		public RealtimeService(IHubContext<MonitorHub> hub) { _hub = hub; }

		public Task Log(string task, LogEntry entry) =>
			_hub.Clients.All.SendAsync("Log", task, entry.ToResponse());

		public Task Task(string task, TaskSnapshot? snapshot) =>
			_hub.Clients.All.SendAsync("Task", task, snapshot?.ToResponse());
	}
}
