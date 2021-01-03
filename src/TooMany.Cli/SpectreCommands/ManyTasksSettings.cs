using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace TooMany.Cli.SpectreCommands
{
	public class ManyTasksSettings: CommandSettings
	{
		[CommandArgument(0, "<TASK...>")]
		[Description("Names of tasks (wildcards are allowed, use '*' for all)")]
		public string[] Names { get; set; } = Array.Empty<string>();

		[CommandOption("-t|--tags <EXPRESSION>")]
		[Description("Tags filter expression (wildcards and logical operations are allowed, ie: \"~(a*|b)&c\")")]
		public string? Tags { get; set; }
	}
}
