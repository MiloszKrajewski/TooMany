using System;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using K4os.Json.Messages.Interfaces;
using Newtonsoft.Json;

namespace TooMany.Actors.Messages
{
	[JsonKnownType("TaskNotFound.v1")]
	public class TaskNotFound: GenericError
	{
		[JsonProperty("task_name")]
		public string? Name { get; set; }

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskNotFound() { }

		public TaskNotFound(IRequest request, string name):
			base(request, $"Task '{name}' could not be found")
		{
			Name = name;
		}
	}
}
