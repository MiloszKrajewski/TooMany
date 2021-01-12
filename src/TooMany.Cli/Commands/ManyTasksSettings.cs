using System;
using System.ComponentModel;
using Spectre.Console.Cli;

namespace TooMany.Cli.Commands
{
	public interface IManyTasksSettings
	{
		string[] Names { get; }
		string? Expression { get; }
	}

	public class ManyTasksSettings: CommandSettings, IManyTasksSettings
	{
		[CommandArgument(0, "<TASK...>")]
		[Description("Names of tasks (wildcards are allowed, use '*' for all)")]
		public string[] Names { get; set; } = Array.Empty<string>();

		[CommandOption("--expression <EXPRESSION>")]
		[Description("Task filter expression (wildcards and logical operations are allowed, ie: \"~(a*|#b)&#c\")")]
		public string? Expression { get; set; }
	}
}
