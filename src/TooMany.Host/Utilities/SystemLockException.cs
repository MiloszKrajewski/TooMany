using System;

namespace TooMany.Host.Utilities
{
	public class SystemLockException: TimeoutException
	{
		public SystemLockException(string message, string kind, Guid guid): base(message)
		{
			Kind = kind;
			Guid = guid;
		}

		public string Kind { get; }

		public Guid Guid { get; }
	}
}
