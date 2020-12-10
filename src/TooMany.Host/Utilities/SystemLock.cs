using System;
using System.Threading;

namespace TooMany.Host.Utilities
{
	public class SystemLock: IDisposable
	{
		private Mutex? _mutex;

		public SystemLock(string kind, Guid guid, TimeSpan timeout)
		{
			var name = $"{kind}/{guid:N}";
			var mutex = new Mutex(false, name, out var created);
			if (!mutex.WaitOne(timeout))
				throw new SystemLockException(
					$"Failed to acquire lock '{name}'", kind, guid);

			_mutex = mutex;
		}

		private void ReleaseUnmanagedResources()
		{
			_mutex?.ReleaseMutex();
			_mutex = null;
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~SystemLock() => ReleaseUnmanagedResources();
	}
}
