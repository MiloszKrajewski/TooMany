using System.Threading;

namespace TooMany.Host.Config
{
	public class ExecutionContext: IExecutionContext
	{
		public CancellationToken Token { get; }

		public ExecutionContext(CancellationToken token) => Token = token;
	}
}
