using System;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("logs", HelpText = "Gets logs of process execution")]
	public class GetLogsCommand: ISingleTaskOptions
	{
		public string Name { get; set; } = string.Empty;

		[Option('t', "tail", Default = null, HelpText = "Tail lines")]
		public int? Tail { get; set; } = null;
	}
}
