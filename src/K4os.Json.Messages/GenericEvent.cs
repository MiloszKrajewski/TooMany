using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class GenericEvent: GenericResponse, IEvent
	{
		public Guid? CommandId { get; set; }

		public GenericEvent(): this(Guid.NewGuid(), null) { }

		public GenericEvent(Guid sagaId, Guid? commandId): base(sagaId) =>
			CommandId = commandId;

		public GenericEvent(Guid sagaId): this(sagaId, null) { }

		public GenericEvent(ICommand command): this(command.SagaId, command.MessageId) { }
	}
}
