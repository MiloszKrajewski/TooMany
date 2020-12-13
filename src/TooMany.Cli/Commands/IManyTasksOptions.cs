using System;
using System.Collections.Generic;
using CommandLine;

namespace TooMany.Cli.Commands
{
	public interface IManyTasksOptions
	{
		[Value(
			0, Required = true, MetaName = "name",
			HelpText = "Names of tasks (wildcards are allowed)")]
		public IEnumerable<string> Names { get; set; }

		[Option(
			't', "tags",
			HelpText =
				"Tags filter expression (wildcards and logical operations are allowed, ie: \"~(a*|b)&c\")")]
		public string? Tags { get; set; }
	}
}
