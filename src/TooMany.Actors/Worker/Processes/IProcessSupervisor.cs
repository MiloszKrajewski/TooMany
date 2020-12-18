using System;
using System.Threading.Tasks;

namespace TooMany.Actors.Worker.Processes
{
	public interface IProcessSupervisor
	{
		Exception? Start();
		Task<bool> Stop();
		Task<bool> Kill();
		Task<int> Wait();
	}
}
