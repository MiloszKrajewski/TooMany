using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using K4os.Json.Messages.Interfaces;
using K4os.Json.TolerantEnum;
using Newtonsoft.Json;
using TooMany.Actors.Worker;
using TooMany.Messages;

namespace TooMany.Actors.Messages
{
	[JsonKnownType("TaskSnapshot.v1")]
	public class TaskSnapshot: GenericResponse
	{
		[JsonProperty("name")]
		public string Name { get; set; } = string.Empty;

		[JsonProperty("directory")]
		public string Directory { get; set; } = string.Empty;

		[JsonProperty("executable")]
		public string Executable { get; set; } = string.Empty;
		
		[JsonProperty("use_shell")]
		public bool UseShell { get; set; }

		[JsonProperty("arguments")]
		public string Arguments { get; set; } = string.Empty;

		[JsonProperty("expected_state"), JsonConverter(typeof(TolerantEnumConverter))]
		public TaskState ExpectedState { get; set; } = TaskState.Stopped;

		[JsonProperty("actual_state"), JsonConverter(typeof(TolerantEnumConverter))]
		public TaskState ActualState { get; set; } = TaskState.Stopped;

		[JsonProperty("started_time")]
		public DateTime? StartedTime { get; set; }

		[JsonProperty("environment")]
		public ImmutableDictionary<string, string?> Environment { get; set; } =
			ImmutableDictionary<string, string?>.Empty;

		[JsonProperty("tags")]
		public ImmutableArray<string> Tags { get; set; } =
			ImmutableArray<string>.Empty;

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskSnapshot() { }
		
		public TaskSnapshot(
			TaskDefinition task, TaskState actualState, DateTime? startedTime)
		{
			InitFields(task, actualState, startedTime);
		}

		public TaskSnapshot(
			IRequest request,
			TaskDefinition task, TaskState actualState, DateTime? startedTime):
			base(request)
		{
			InitFields(task, actualState, startedTime);
		}
		
		private void InitFields(TaskDefinition task, TaskState actualState, DateTime? startedTime)
		{
			Name = task.Name;
			Directory = task.Directory;
			Executable = task.Executable;
			UseShell = task.UseShell;
			Arguments = task.Arguments;
			ExpectedState = task.ExpectedState;
			ActualState = actualState;
			StartedTime = startedTime;
			Environment = task.Environment.ToImmutableDictionary();
			Tags = task.Tags.ToImmutableArray();
		}
	}

	[JsonKnownType("ManyTasksSnapshot.v1")]
	public class ManyTasksSnapshot: GenericResponse
	{
		[JsonProperty("tasks")]
		public TaskSnapshot[] Tasks { get; set; } = Array.Empty<TaskSnapshot>();

		[JsonConstructor, Obsolete("Serialization only")]
		protected ManyTasksSnapshot() { }

		public ManyTasksSnapshot(IRequest request, IEnumerable<TaskSnapshot> snapshots):
			base(request)
		{
			Tasks = snapshots.ToArray();
		}
	}
	
	[JsonKnownType("TaskLog.v1")]
	public class TaskLog: GenericResponse
	{
		[JsonProperty("messages")]
		public LogEntry[] Messages { get; set; } = Array.Empty<LogEntry>();

		[JsonConstructor, Obsolete("Serialization only")]
		public TaskLog() { }

		public TaskLog(IRequest request, LogEntry[] messages): base(request) =>
			Messages = messages;
	}
}
