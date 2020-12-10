using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class GenericMessage: IMessage
	{
		public Guid SagaId { get; protected set; }
		public Guid MessageId { get; protected set; }
		public DateTime Timestamp { get; protected set; }

		public GenericMessage(Guid sagaId) =>
			(SagaId, MessageId, Timestamp) = (sagaId, Guid.NewGuid(), DateTime.UtcNow);

		public GenericMessage(): this(Guid.NewGuid()) { }

		public GenericMessage(IMessage other): this(other.SagaId) { }
	}
}
