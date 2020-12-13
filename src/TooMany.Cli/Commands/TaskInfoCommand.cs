using System;
using System.Collections.Generic;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("info", HelpText = "Provides full information about tasks")]
	public class TaskInfoCommand: IManyTasksOptions
	{
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
		
		public string? Tags { get; set; } = null;
	}
}
