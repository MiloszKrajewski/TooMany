// using System;
// using System.Collections.Generic;
// using System.Linq;
// using CommandLine;
//
// namespace TooMany.Cli.Commands
// {
// 	[Verb("remove", HelpText = "Removed task")]
// 	public class RemoveTaskCommand: IManyTasksOptions
// 	{
// 		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
// 		
// 		public string? Tags { get; set; } = null;
// 	
// 		[Option('f', "force", HelpText = "Confirms removal when using wildcards")]
// 		public bool Force { get; set; }
//
// 	}
// }
