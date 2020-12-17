using System;
using System.Collections.Generic;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("monitor", HelpText = "monitors execution of tasks")]
	public class MonitorCommand: IManyTasksOptions
	{
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();

		public string? Tags { get; set; } = null;
	}
}
