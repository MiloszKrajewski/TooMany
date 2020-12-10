namespace HttpRemoting.Data
{
	public static class JsonResponseExtensions
	{
		public static object? Resolve(IJsonResponse response)
		{
			if (response.Error is null)
				return response.Result;

			throw new HttpRemotingException(response.Error);
		}

		public static T Resolve<T>(IJsonResponse<T> response)
		{
			if (response.Error is null)
				return response.Result;

			throw new HttpRemotingException(response.Error);
		}
	}
}
