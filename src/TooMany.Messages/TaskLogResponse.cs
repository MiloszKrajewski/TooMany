using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TooMany.Messages
{
	public enum LogChannel
	{
		StdOut,
		StdErr,
	}
	
	public class LogEntryResponse
	{
		[JsonProperty("channel"), JsonConverter(typeof(StringEnumConverter))]
		public LogChannel Channel { get; set; }
		public DateTime Timestamp { get; set; }
		public string? Text { get; set; }
	}
}
