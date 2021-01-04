using System;

namespace HttpRemoting.Server.Tracing
{
	internal class ActionStats
	{
		public string ActionName { get; }
		public int ConcurrentCalls { get; private set; }
		public long TotalCalls { get; private set; }
		public long TotalFailed { get; private set; }
		public TimeSpan TotalTime { get; private set; }
		public int MaxConcurrentCalls { get; private set; }

		public ActionStats(string actionName)
		{
			ActionName = actionName;
		}

		public void Enter()
		{
			lock (this)
			{
				ConcurrentCalls++;
				MaxConcurrentCalls = Math.Max(ConcurrentCalls, MaxConcurrentCalls);
			}
		}

		public T Leave<T>(
			Exception? exception, TimeSpan elapsed, Func<TimeSpan, ActionStats, T> map)
		{
			lock (this)
			{
				if (exception != null)
					TotalFailed++;
				TotalCalls++;
				TotalTime = TotalTime.Add(elapsed);
				var result = map(elapsed, this);
				ConcurrentCalls--;
				return result;
			}
		}
	}
}
