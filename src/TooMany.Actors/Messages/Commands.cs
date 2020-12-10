using System;
using System.Collections.Generic;
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

		[JsonProperty("arguments")]
		public string Arguments { get; set; } = string.Empty;
		
		[JsonProperty("directory")]
		public string Directory { get; set; } = string.Empty;

		[JsonProperty("environment")]
		public Dictionary<string, string?> Environment { get; set; } =
			new Dictionary<string, string?>();
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
	public class GetTasks: GenericRequest { }
}
