using System;
using System.Net;
using Newtonsoft.Json;

namespace HttpRemoting.Data
{
	public sealed class JsonError: IHttpRemotingError
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("error_code")]
		public string? ErrorCode { get; set; }

		[JsonProperty("status_code")]
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

		[JsonConstructor]
		public JsonError()
		{
			Message = null!;
		}

		public JsonError(HttpStatusCode statusCode, string? errorCode, string message)
		{
			StatusCode = statusCode;
			ErrorCode = errorCode;
			Message = message;
		}

		public JsonError(HttpStatusCode statusCode, string? errorCode, Exception exception):
			this(statusCode, errorCode, exception.Message) { }

		public JsonError(IHttpRemotingError error):
			this(error.StatusCode, error.ErrorCode, error.Message) { }

		public JsonError(Exception exception):
			this(HttpStatusCode.InternalServerError, null, exception.Message) { }
	}
}
