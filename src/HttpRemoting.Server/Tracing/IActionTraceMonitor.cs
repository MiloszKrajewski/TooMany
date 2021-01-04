using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpRemoting.Server.Tracing
{
	public interface IActionTraceMonitor
	{
		void Enter(ActionExecutingContext context);
		void Leave(ActionExecutedContext context);
	}
}
