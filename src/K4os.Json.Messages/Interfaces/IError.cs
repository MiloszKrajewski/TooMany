using Newtonsoft.Json;

namespace K4os.Json.Messages.Interfaces
{
	public interface IError: IResponse
	{
		[JsonProperty("error_message")]
		string ErrorMessage { get; set; }
	}
}
