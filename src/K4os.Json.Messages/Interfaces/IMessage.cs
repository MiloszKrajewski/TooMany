using System;
using Newtonsoft.Json;

namespace K4os.Json.Messages.Interfaces
{
	public interface IMessage
	{
		[JsonProperty("saga_id")]
		Guid SagaId { get; }

		[JsonProperty("message_id")]
		Guid MessageId { get; }

		[JsonProperty("message_timestamp")]
		DateTime Timestamp { get; }
	}
}
