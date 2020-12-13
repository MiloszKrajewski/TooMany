using System;
using System.Collections.Generic;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("tag", HelpText = "Manage task tags")]
	public class ApplyTagsCommand: IManyTasksOptions
	{
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();

		public string? Tags { get; set; } = null;

		[Option('s', "set", HelpText = "Flags to set")]
		public IEnumerable<string> Set { get; set; } = Array.Empty<string>();

		[Option('c', "clear", HelpText = "Flags to clear")]
		public IEnumerable<string> Reset { get; set; } = Array.Empty<string>();
	}
}
