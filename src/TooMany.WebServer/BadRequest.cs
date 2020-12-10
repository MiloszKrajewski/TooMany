using System;
using System.Net;
using HttpRemoting.Data;

namespace TooMany.WebServer
{
	public class BadRequest: Exception, IHttpRemotingError
	{
		public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
		public string? ErrorCode => null;
	}
}
