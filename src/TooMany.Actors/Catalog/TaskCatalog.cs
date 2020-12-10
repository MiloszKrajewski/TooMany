using System;
using System.Collections.Generic;
using System.Linq;
using K4os.Json.KnownTypes;
using Newtonsoft.Json;
using TooMany.Actors.Messages;

namespace TooMany.Actors.Catalog
{
	[JsonKnownType("TaskCatalog.v1")]
	public class TaskCatalog: ICloneable
	{
		public static readonly StringComparer Comparer = 
			StringComparer.InvariantCultureIgnoreCase;

		public Dictionary<string, TaskCatalogEntry> Tasks { get; set; } =
			new Dictionary<string, TaskCatalogEntry>(Comparer);

		public TaskCatalog() { }

		public TaskCatalog(TaskCatalog other)
		{
			Tasks.CopyFrom(other.Tasks.Select(kv => (kv.Key, kv.Value.TypedClone())));
		}

		public virtual object Clone() => new TaskCatalog(this);
	}

	[JsonKnownType("TaskCatalogEntry.v1")]
	public class TaskCatalogEntry: ICloneable
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;

		[JsonConstructor, Obsolete("Serialization only")]
		protected TaskCatalogEntry() { }

		public TaskCatalogEntry(TaskCatalogEntry other)
		{
			Id = other.Id;
			Name = other.Name;
		}

		public TaskCatalogEntry(TaskCreated e)
		{
			Id = e.Id;
			Name = e.Name;
		}

		public virtual object Clone() => new TaskCatalogEntry(this);
	}
}
