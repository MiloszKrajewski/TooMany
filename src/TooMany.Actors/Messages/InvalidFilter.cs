using System;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using K4os.Json.Messages.Interfaces;
using Newtonsoft.Json;

namespace TooMany.Actors.Messages
{
	[JsonKnownType("InvalidFilter.v1")]
	public class InvalidFilter: GenericError
	{
		[JsonProperty("filter")]
		public string? Filter { get; set; }

		[JsonConstructor, Obsolete("Serialization only")]
		protected InvalidFilter() { }

		public InvalidFilter(IRequest request, string? filter):
			base(request, $"Filter '{filter ?? "<null>"}' is not valid")
		{
			Filter = filter;
		}
	}
}
