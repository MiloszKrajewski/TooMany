using Microsoft.Extensions.Logging;
using Proto;
using TooMany.Actors.Worker;

namespace TooMany.Actors.Tools
{
	public class DebugLoggingMiddleware: LoggingMiddleware
	{
		public DebugLoggingMiddleware(ILoggerFactory? loggerFactory): base(loggerFactory) { }

		protected override bool SuppressTrace(IInfoContext context, object message) =>
			message switch {
				LogEntry _ => true,
				_ => base.SuppressTrace(context, message)
			};
	}
}
