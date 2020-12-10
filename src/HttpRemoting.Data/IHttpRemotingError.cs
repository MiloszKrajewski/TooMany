using System.Net;

namespace HttpRemoting.Data
{
	public interface IHttpRemotingError
	{
		HttpStatusCode StatusCode { get; }
		string? ErrorCode { get; }
		string Message { get; }
	}
}
