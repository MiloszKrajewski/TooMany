using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Spectre.Console.Cli;

namespace Playground
{
	class Program
	{
		static void Main(string[] args)
		{
			var testArgs = new[] {
				"run",
				"-e", "A=7",
				"-e", "B=10",
				"a", "b", "-e x=2",
				// "a", "b", "c", "d", "-e x",
				"--",
				"/c", "set && pause"
			};
			testArgs = args;
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
			public string[] Executable { get; set; } = null!;

			[CommandOption("-d|--details")]
			public bool Details { get; set; }

			[CommandOption("-e <VAR=VAL>")]
			public ILookup<string, string> Environment { get; set; } =
				Enumerable.Empty<string>().ToLookup(x => x);
		}

		public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
		{
			Console.WriteLine(JsonConvert.SerializeObject(settings));
			var remaining = new {
				Parsed = context.Remaining.Parsed.ToDictionary(g => g.Key, g => g.ToArray()),
				Raw = context.Remaining.Raw.ToArray()
			};
			Console.WriteLine(JsonConvert.SerializeObject(remaining));
			return 0;
		}
	}
}
