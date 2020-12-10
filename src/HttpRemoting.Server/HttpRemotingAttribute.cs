using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace HttpRemoting.Server
{
	public class HttpRemotingAttribute: Attribute, IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			// do nothing
		}

		public void OnActionExecuted(ActionExecutedContext context)
		{
			var services = context.HttpContext.RequestServices;
			var builder = services?.GetService<IHttpRemotingResponseBuilder>();
			builder?.Execute(context);
		}
	}
}
