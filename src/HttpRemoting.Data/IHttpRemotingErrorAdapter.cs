using System;

namespace HttpRemoting.Data
{
	public interface IHttpRemotingErrorAdapter
	{
		IHttpRemotingError? ToJsonError(Exception exception);
		Exception? ToException(IHttpRemotingError error);
	}
}
