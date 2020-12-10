using System;
using System.Data;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class GenericResponse: GenericMessage, IResponse
	{
		public Guid? RequestId { get; set; }

		public GenericResponse() { }

		public GenericResponse(Guid sagaId): base(sagaId) { }

		public GenericResponse(IMessage other): base(other) { RequestId = other.MessageId; }
	}
}
