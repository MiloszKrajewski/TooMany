using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace HttpRemoting.Server.Tracing
{
	public class ActionTraceAttribute: Attribute, IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			context.HttpContext.RequestServices
				.GetService<IActionTraceMonitor>()?.Enter(context);
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			context.HttpContext.RequestServices
				.GetService<IActionTraceMonitor>()?.Leave(context);
		}
	}
}
