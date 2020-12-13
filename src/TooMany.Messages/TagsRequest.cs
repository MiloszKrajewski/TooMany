using System.Collections.Generic;
using Newtonsoft.Json;

namespace TooMany.Messages
{
	public class TagsRequest
	{
		[JsonProperty("tags")]
		public List<string?>? Tags { get; set; }
	}
}
