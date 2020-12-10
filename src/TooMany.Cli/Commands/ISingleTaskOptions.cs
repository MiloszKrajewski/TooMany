using System;
using CommandLine;

namespace TooMany.Cli.Commands
{
	public interface ISingleTaskOptions
	{
		[Value(0, Required = true, HelpText = "Name of task")]
		public string Name { get; set; }
	}
}
