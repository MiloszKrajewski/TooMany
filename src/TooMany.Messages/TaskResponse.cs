using System;
using System.Collections.Generic;
using K4os.Json.TolerantEnum;
using Newtonsoft.Json;

namespace TooMany.Messages
{
	public class TaskResponse
	{
		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("executable")]
		public string Executable { get; set; } = string.Empty;
		
		[JsonProperty("use_shell")]
		public bool UseShell { get; set; }

		[JsonProperty("arguments")]
		public string Arguments { get; set; } = string.Empty;

		[JsonProperty("directory")]
		public string Directory { get; set; } = string.Empty;
		
		[JsonProperty("expected_state"), JsonConverter(typeof(TolerantEnumConverter))]
		public TaskState ExpectedState { get; set; }
		
		[JsonProperty("actual_state"), JsonConverter(typeof(TolerantEnumConverter))]
		public TaskState ActualState { get; set; }
		
		[JsonProperty("started_time")]
		public DateTime? StartedTime { get; set; }
		
		[JsonProperty("environment")]
		public Dictionary<string, string?> Environment { get; set; } =
			new Dictionary<string, string?>();
		
		[JsonProperty("tags")]
		public List<string> Tags { get; set; } =
			new List<string>();
	}
}
