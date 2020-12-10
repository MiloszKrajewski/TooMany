using System;
using K4os.Json.KnownTypes;
using K4os.Json.Messages.Interfaces;
using Newtonsoft.Json;
using TooMany.Actors.Worker;
using TooMany.Messages;

namespace TooMany.Actors.Messages
{
	public enum ActionType
	{
		None,
		Created,
		Updated,
		Deleted
	}

	[JsonKnownType("TaskAction.v1")]
	public abstract class TaskAction: TaskSnapshot, IEvent
	{
		[JsonIgnore]
		public abstract ActionType ActionType { get; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskAction() { Id = null!; }

		protected TaskAction(
			IRequest request, 
			string id, TaskDefinition task, TaskState actualState, DateTime? startedTime):
			base(request, task, actualState, startedTime)
		{
			Id = id;
		}
	}

	[JsonKnownType("TaskCreated.v1")]
	public class TaskCreated: TaskAction
	{
		public override ActionType ActionType => ActionType.Created;

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskCreated() { }

		public TaskCreated(
			IRequest request, 
			string id, TaskDefinition definition, TaskState actualState, DateTime? startedTime):
			base(request, id, definition, actualState, startedTime) { }
	}

	[JsonKnownType("TaskUpdated.v1")]
	public class TaskUpdated: TaskAction
	{
		public override ActionType ActionType => ActionType.Updated;

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskUpdated() { }

		public TaskUpdated(
			IRequest request, 
			string id, TaskDefinition definition, TaskState actualState, DateTime? startedTime):
			base(request, id, definition, actualState, startedTime) { }
	}

	[JsonKnownType("TaskRemoved.v1")]
	public class TaskRemoved: TaskAction
	{
		public override ActionType ActionType => ActionType.Deleted;

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskRemoved() { }

		public TaskRemoved(
			IRequest request, 
			string id, TaskDefinition definition, TaskState actualState, DateTime? startedTime):
			base(request, id, definition, actualState, startedTime) { }
	}
}
