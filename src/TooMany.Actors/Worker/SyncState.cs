using System;
using K4os.Json.KnownTypes;
using K4os.Json.Messages;
using Newtonsoft.Json;

namespace TooMany.Actors.Worker
{
	[JsonKnownType("SyncState.v1")]
	public class SyncState: GenericCommand
	{
		[JsonProperty("sync_id")]
		public Guid SyncId { get; set; }

		[JsonConstructor, Obsolete("Serialization only")]
		protected SyncState() { }

		public SyncState(Guid syncId) { SyncId = syncId; }
	}
}
