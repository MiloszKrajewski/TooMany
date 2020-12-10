using System.Threading.Tasks;
using Proto;

namespace TooMany.Actors.Tools
{
	public interface IReceiverMiddleware
	{
		Task Handle(Receiver next, IReceiverContext context, MessageEnvelope envelope);
	}
}
