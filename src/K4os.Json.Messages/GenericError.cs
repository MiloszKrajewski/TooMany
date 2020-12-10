using System;
using K4os.Json.Messages.Interfaces;
using Newtonsoft.Json;

namespace K4os.Json.Messages
{
	public class GenericError: GenericResponse, IError
	{
		public string ErrorMessage { get; set; }

		public GenericError() { }

		public GenericError(string errorMessage) { ErrorMessage = errorMessage; }

		public GenericError(Guid sagaId, string errorMessage): base(sagaId)
		{
			ErrorMessage = errorMessage;
		}

		public GenericError(IMessage other, string errorMessage):
			this(other.SagaId, errorMessage) { }
	}
}
