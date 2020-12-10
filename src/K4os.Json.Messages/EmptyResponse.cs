using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class EmptyResponse: GenericResponse
	{
		public EmptyResponse() { }

		public EmptyResponse(Guid sagaId): base(sagaId) { }

		public EmptyResponse(IMessage other): base(other) { }
	}
}
