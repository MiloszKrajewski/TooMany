using System.Threading;

namespace TooMany.Host.Config
{
	public interface IExecutionContext
	{
		CancellationToken Token { get; }
	}
}
