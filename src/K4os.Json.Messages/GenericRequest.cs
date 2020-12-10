using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class GenericRequest: GenericMessage, IRequest
	{
		public GenericRequest() { }

		public GenericRequest(Guid sagaId): base(sagaId) { }

		public GenericRequest(IMessage other): base(other) { }
	}

	public class GenericRequest<TResponse>: GenericRequest, IRequest<TResponse>
		where TResponse: IResponse
	{
		public GenericRequest() { }

		public GenericRequest(Guid sagaId): base(sagaId) { }

		public GenericRequest(IMessage other): base(other) { }
	}
}
