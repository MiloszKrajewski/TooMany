using System;
using Newtonsoft.Json;

namespace K4os.Json.Messages.Interfaces
{
	public interface IResponse: IMessage
	{
		[JsonProperty("request_id")]
		Guid? RequestId { get; }
	}
}
