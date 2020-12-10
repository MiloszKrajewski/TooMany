using System;

namespace HttpRemoting.Data
{
	public class DefaultHttpRemotingErrorAdapter: IHttpRemotingErrorAdapter
	{
		public static readonly DefaultHttpRemotingErrorAdapter Instance =
			new DefaultHttpRemotingErrorAdapter();

		public virtual IHttpRemotingError? ToJsonError(Exception exception) =>
			exception is IHttpRemotingError e ? e : new JsonError(exception);

		public virtual Exception? ToException(IHttpRemotingError error) =>
			error is Exception e ? e : new HttpRemotingException(error);
	}
}
