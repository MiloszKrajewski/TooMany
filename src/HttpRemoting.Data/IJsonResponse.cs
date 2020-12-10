using Newtonsoft.Json;

namespace HttpRemoting.Data
{
	public interface IJsonResponse
	{
		object? Result { get; }
		IHttpRemotingError? Error { get; }
	}
	
	public interface IJsonResponse<out T>: IJsonResponse
	{
		new T Result { get; }
	}

}
