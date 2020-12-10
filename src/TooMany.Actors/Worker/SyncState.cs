using K4os.Json.KnownTypes;
using K4os.Json.Messages;

namespace TooMany.Actors.Worker
{
	[JsonKnownType("SyncState.v1")]
	public class SyncState: GenericCommand { }
}
