using System;
using System.Linq;
using Newtonsoft.Json;
using Spectre.Cli;

namespace Playground
{
	class Program
	{
		static void Main(string[] args)
		{
			var testArgs = new[] {
				"run",
				"cmd",
				"-e", "A=7",
				"-e", "B=10",
				"--",
				"cmd", "/c", "set && pause"
			};
			var app = new CommandApp();
			app.Configure(config => config.AddCommand<RunCommand>("run"));
			app.Run(testArgs);
		}
	}

	public class RunCommand: Command<RunCommand.Settings>
	{
		public class Settings: CommandSettings
		{
			[CommandArgument(0, "<EXECUTABLE>")]
			public string Executable { get; set; } = null!;

			[CommandOption("-e <VAR=VAL>")]
			public string[] Environment { get; set; } = Array.Empty<string>();
		}

		public override int Execute(CommandContext context, Settings settings)
		{
			Console.WriteLine(JsonConvert.SerializeObject(settings));
			Console.WriteLine(JsonConvert.SerializeObject(new {
				Parsed = context.Remaining.Parsed.ToArray(),
				Raw = context.Remaining.Raw.ToArray()
			}));
			return 0;
		}
	}
}
