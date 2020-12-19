using System;
using System.Net;

namespace HttpRemoting.Data
{
	public class HttpRemotingException: Exception, IHttpRemotingError
	{
		private const string UnknownError = "Unknown error";
		
		public HttpStatusCode StatusCode { get; }
		public string? ErrorCode { get; }

		public HttpRemotingException(
			HttpStatusCode statusCode, string? errorCode, string? message):
			base(message ?? UnknownError)
		{
			StatusCode = statusCode;
			ErrorCode = errorCode;
		}

		public HttpRemotingException(
			HttpStatusCode statusCode, string? errorCode, string? message, 
			Exception innerException):
			base(message ?? UnknownError, innerException)
		{
			StatusCode = statusCode;
			ErrorCode = errorCode;
		}

		public HttpRemotingException(IHttpRemotingError error): 
			this(error.StatusCode, error.ErrorCode, error.Message) { }
	}
}
