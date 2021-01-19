using System;
using System.Collections.Immutable;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using Newtonsoft.Json;

namespace TooMany.Actors.Messages
{
	public abstract class TaskRef: GenericRequest
	{
		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;
	}
	
	[JsonKnownType("DefineTask.v1")]
	public class DefineTask: TaskRef
	{
		[JsonProperty("executable")]
		public string Executable { get; set; } = string.Empty;
		
		[JsonProperty("use_shell")]
		public bool UseShell { get; set; }

		[JsonProperty("arguments")]
		public string Arguments { get; set; } = string.Empty;
		
		[JsonProperty("directory")]
		public string Directory { get; set; } = string.Empty;

		[JsonProperty("environment")]
		public ImmutableDictionary<string, string?> Environment { get; set; } =
			ImmutableDictionary<string, string?>.Empty;
		
		[JsonProperty("tags")]
		public ImmutableArray<string> Tags { get; set; } =
			ImmutableArray<string>.Empty;
	}

	[JsonKnownType("RemoveTask.v1")]
	public class RemoveTask: TaskRef { }

	[JsonKnownType("StartTask.v1")]
	public class StartTask: TaskRef
	{
		[JsonProperty("force")]
		public bool? Force { get; set; }
	}

	[JsonKnownType("StopTask.v1")]
	public class StopTask: TaskRef { }

	[JsonKnownType("GetTask.v1")]
	public class GetTask: TaskRef { }
	
	[JsonKnownType("GetLog.v1")]
	public class GetLog: TaskRef { }

	[JsonKnownType("GetTasks.v1")]
	public class GetTasks: GenericRequest
	{
		[JsonProperty("filter")]
		public string? Filter { get; set; }
		
		public GetTasks(string? filter = null)
		{
			Filter = filter;
		}
	}

	[JsonKnownType("SetTags.v1")]
	public class SetTags: TaskRef
	{
		[JsonProperty("tags")]
		public ImmutableArray<string> Tags { get; set; } =
			ImmutableArray<string>.Empty;
	}
}
