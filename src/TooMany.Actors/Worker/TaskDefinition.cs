using System;
using System.Collections.Generic;
using System.Linq;
using K4os.Json.KnownTypes;
using K4os.Json.TolerantEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TooMany.Actors.Messages;
using TooMany.Messages;

namespace TooMany.Actors.Worker
{
	[JsonKnownType("TaskDefinition.v1")]
	public class TaskDefinition: ICloneable
	{
		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("directory")]
		public string Directory { get; set; } = string.Empty;

		[JsonProperty("executable")]
		public string Executable { get; set; } = string.Empty;

		[JsonProperty("arguments")]
		public string Arguments { get; set; } = string.Empty;

		[JsonProperty("expected_state"), JsonConverter(typeof(TolerantEnumConverter))]
		public TaskState ExpectedState { get; set; } = TaskState.Stopped;

		[JsonProperty("environment")]
		public Dictionary<string, string?> Environment { get; set; } =
			new Dictionary<string, string?>();

		public TaskDefinition() { }

		public TaskDefinition(TaskDefinition other)
		{
			Name = other.Name;
			Directory = other.Directory;
			Executable = other.Executable;
			Arguments = other.Arguments;
			ExpectedState = other.ExpectedState;
			Environment = other.Environment.ToDictionary();
		}

		public virtual object Clone() => new TaskDefinition(this);
	}
}
