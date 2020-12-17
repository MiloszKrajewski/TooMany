using System.Threading.Tasks;
using TooMany.Actors.Messages;
using TooMany.Actors.Worker;

namespace TooMany.Actors
{
	public interface IRealtimeService
	{
		public Task Log(string task, LogEntry message);

		public Task Task(string task, TaskSnapshot? snapshot);
	}
}
