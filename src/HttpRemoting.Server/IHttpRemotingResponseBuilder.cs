using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HttpRemoting.Server
{
	public interface IHttpRemotingResponseBuilder
	{
		void Execute(ActionExecutedContext context);
	}
}
