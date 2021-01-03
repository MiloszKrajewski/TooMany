// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using System.Threading;
// using System.Threading.Tasks;
// using K4os.RoutR.Abstractions;
// using Microsoft.Extensions.Logging;
// using TooMany.Cli.Commands;
// using TooMany.Cli.UserInterface;
// using TooMany.Messages;
//
// namespace TooMany.Cli.Handlers
// {
// 	public class DefineTaskHandler: HostCommandHandler, ICommandHandler<DefineTaskCommand>
// 	{
// 		private static readonly Regex KeyValuePattern = new Regex("(?<key>[^=]+)(=(?<value>.*))?");
//
// 		public DefineTaskHandler(
// 			ILoggerFactory loggerFactory,
// 			IHostInterface hostInterface):
// 			base(loggerFactory, hostInterface) { }
//
// 		public async Task Handle(DefineTaskCommand command, CancellationToken token)
// 		{
// 			var request = new TaskRequest {
// 				Executable = command.Executable,
// 				Arguments = ToArguments(command.Arguments),
// 				Directory = command.Directory,
// 				Environment = ToEnvironment(command.Environment),
// 			};
// 			var response = await Host.CreateTask(command.Name, request);
//
// 			Presentation.TaskDetails(response);
// 		}
//
// 		private static string ToArguments(IEnumerable<string> arguments) =>
// 			string.Join(' ', arguments);
//
// 		private static Dictionary<string, string?> ToEnvironment(
// 			IEnumerable<string> keyValuePairs) =>
// 			keyValuePairs
// 				.Select(ParseKeyValuePair)
// 				.ToDictionary(kv => kv.Key, kv => kv.Value);
//
// 		private static (string Key, string? Value) ParseKeyValuePair(string pair)
// 		{
// 			var m = KeyValuePattern.Match(pair);
// 			if (!m.Success)
// 				throw new ArgumentException(
// 					$"'{pair}' does not look like valid key/value pair");
//
// 			var keyGroup = m.Groups["key"];
// 			var valueGroup = m.Groups["value"];
// 			var key = keyGroup.Value;
// 			var value = valueGroup.Success ? valueGroup.Value : null;
//
// 			return (key, value);
// 		}
// 	}
// }
