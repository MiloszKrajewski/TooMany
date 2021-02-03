using System;
using System.ComponentModel;
using System.Linq;
using Spectre.Console.Cli;

namespace TooMany.Cli.Commands
{
	public interface IManyTasksSettings
	{
		string[] Tasks { get; }
	}

	public class ManyTasksSettings: CommandSettings, IManyTasksSettings
	{
		[CommandArgument(0, "<TASK...>")]
		[Description("Names of tasks (wildcards and logical operations are allowed, use '*' for all)")]
		public string[] Tasks { get; set; } = Array.Empty<string>();
	}
}
