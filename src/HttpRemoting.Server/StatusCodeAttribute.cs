using System;
using System.Net;

namespace HttpRemoting.Server
{
	public class StatusCodeAttribute: Attribute
	{
		public HttpStatusCode StatusCode { get; }

		public StatusCodeAttribute(HttpStatusCode statusCode) =>
			StatusCode = statusCode;

		public StatusCodeAttribute(int statusCode) =>
			StatusCode = (HttpStatusCode) statusCode;
	}
}
