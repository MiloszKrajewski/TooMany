using System.Collections.Generic;
using TooMany.Actors.Worker;

namespace TooMany.Actors.Tools
{
	internal class SlidingLog
	{
		private readonly Queue<LogEntry> _logHistory = new Queue<LogEntry>();
		private readonly int _logHistorySize;

		public SlidingLog(int logHistorySize) { _logHistorySize = logHistorySize; }

		public void Add(LogEntry entry)
		{
			_logHistory.Enqueue(entry);
			TrimLog();
		}

		private void TrimLog()
		{
			while (_logHistory.Count > _logHistorySize)
				_logHistory.Dequeue();
		}

		public LogEntry[] Snapshot() => _logHistory.ToArray();
	}
}
