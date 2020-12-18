using System;

namespace TooMany.Actors.Worker
{
	public class LogEntry
	{
		public bool Error { get; set; }
		public DateTime Timestamp { get; set; }
		public string Text { get; set; }

		public LogEntry(bool error, string text)
		{
			Error = error;
			Text = text;
			Timestamp = DateTime.UtcNow;
		}
	}
}
