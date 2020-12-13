using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("list", HelpText = "List tasks")]
	public class ListTaskCommand: IManyTasksOptions
	{
		[Value(
			0, Required = false, MetaName = "name",
			HelpText = "Names of tasks (wildcards are allowed)")]
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
		
		public string? Tags { get; set; } = null;
	}
}
