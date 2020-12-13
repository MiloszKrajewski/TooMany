using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TooMany.Messages
{
	public class TaskRequest
	{
		[JsonProperty("executable")]
		public string? Executable { get; set; }

		[JsonProperty("arguments")]
		public string? Arguments { get; set; }

		[JsonProperty("directory")]
		public string? Directory { get; set; }

		[JsonProperty("environment")]
		public Dictionary<string, string?>? Environment { get; set; }

		[JsonProperty("tags")]
		public List<string?>? Tags { get; set; }
	}
}
