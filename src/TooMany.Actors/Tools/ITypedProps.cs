using Proto;

namespace TooMany.Actors.Tools
{
	public interface ITypedProps<T> where T: IActor
	{
		Props Props { get; }
	}
}
