using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Spectre.Console;

namespace TooMany.Cli.Commands
{
	[Verb("define", HelpText = "Defines or redefines task")]
	public class DefineTaskCommand: ISingleTaskOptions
	{
		public string Name { get; set; } = string.Empty;

		[Option('e', "environment", HelpText = "Environment variable (in: KEY=VALUE format)")]
		public IEnumerable<string> Environment { get; set; } = Array.Empty<string>();

		[Option('d', "directory", HelpText = "Working directory")]
		public string Directory { get; set; } = string.Empty;

		[Option('x', "executable", Required = true, HelpText = "Executable")]
		public string Executable { get; set; } = string.Empty;

		[Option('a', "arguments", HelpText = "Arguments")]
		public IEnumerable<string> Arguments { get; set; } = Array.Empty<string>();
	}
}
