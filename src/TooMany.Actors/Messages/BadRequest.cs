using System;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using K4os.Json.Messages.Interfaces;
using Newtonsoft.Json;

namespace TooMany.Actors.Messages
{
	[JsonKnownType("BadRequest.v1")]
	public class BadRequest: GenericError
	{
		[JsonProperty("task_name")]
		public string? Name { get; set; }

		[JsonConstructor, Obsolete("Serialization only")]
		protected BadRequest() { }

		public BadRequest(IRequest request, string name, string message):
			base(request, message)
		{
			Name = name;
		}
	}
}
