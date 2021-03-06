using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console.Cli;
using TooMany.Cli.UserInterface;
using TooMany.Messages;

namespace TooMany.Cli.Commands
{
	[Description("Define, or redefine a task")]
	public class DefineTaskCommand: HostCommand<DefineTaskCommand.Settings>
	{
		private static readonly Regex KeyValuePattern =
			new Regex("(?<key>[^=]+)(=(?<value>.*))?");

		public class Settings: CommandSettings
		{
			[CommandArgument(0, "<TASK>")]
			[Description("Name of tasks")]
			public string Name { get; set; } = null!;

			[CommandOption("-x|--direct-execute")]
			[Description("Do not use shell to execute command")]
			public bool DirectExecute { get; set; }

			[CommandOption("-e|--environment <KEY=VALUE>")]
			[Description("Environment variable")]
			public string[] Environment { get; set; } = Array.Empty<string>();

			[CommandOption("-d|--directory <DIRECTORY>")]
			[Description("Working directory")]
			public string Directory { get; set; } = string.Empty;

			[CommandOption("-t|--tag <TAG>")]
			[Description("Assign tag to task")]
			public string[] Tags { get; set; } = Array.Empty<string>();

			[CommandArgument(1, "[ARGUMENTS]")]
			[Description("Arguments (executable and arguments)")]
			public string[] Arguments { get; set; } = Array.Empty<string>();
		}

		public DefineTaskCommand(IHostInterface host, IRawArguments args): base(host, args) { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			ShowUnknownOptions(context);
			// ShowIgnoredArguments(context);

			var arguments = settings.Arguments.Concat(Args.Tail.NotNull()).ToArray();
			var directory = FullDirectoryPath(settings.Directory);
			var useShell = !settings.DirectExecute;

			if (arguments.Length <= 0)
				throw new ArgumentException("At least 1 argument is needed");

			var request = new TaskRequest {
				Executable = arguments[0],
				UseShell = useShell,
				Arguments = ToArguments(arguments[1..]),
				Directory = directory,
				Tags = ExpandTags(settings.Tags).ToList().NullIfEmpty(),
				Environment = ToEnvironment(settings.Environment),
			};

			var response = await Host.CreateTask(settings.Name, request)
				.WithSpinner("Updating task definition...");

			Presentation.TaskDetails(response);

			return 0;
		}

		private static string FullDirectoryPath(string? directory) =>
			!string.IsNullOrWhiteSpace(directory)
				? Path.GetFullPath(directory)
				: directory.NotNull();

		private static string ToArguments(IEnumerable<string> arguments) =>
			string.Join(' ', arguments.Select(ToArgument));

		private static string ToArgument(string argument) => argument.Quote();

		private static Dictionary<string, string?> ToEnvironment(
			IEnumerable<string> keyValuePairs) =>
			keyValuePairs
				.Select(ParseKeyValuePair)
				.ToDictionary(kv => kv.Key, kv => kv.Value);

		private static (string Key, string? Value) ParseKeyValuePair(string pair)
		{
			var m = KeyValuePattern.Match(pair);
			if (!m.Success)
				throw new ArgumentException(
					$"'{pair}' does not look like valid key/value pair");

			var keyGroup = m.Groups["key"];
			var valueGroup = m.Groups["value"];
			var key = keyGroup.Value;
			var value = valueGroup.Success ? valueGroup.Value : null;

			return (key, value);
		}
	}
}
