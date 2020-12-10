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
	}
}
