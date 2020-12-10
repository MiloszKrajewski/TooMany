using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class GenericCommand: GenericMessage, ICommand
	{
		public GenericCommand() { }

		public GenericCommand(Guid sagaId): base(sagaId) { }

		public GenericCommand(IMessage other): base(other) { }
	}
}
