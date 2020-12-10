using System;
using System.Collections.Generic;
using CommandLine;

namespace TooMany.Cli.Commands
{
	[Verb("start", HelpText = "Starts or restarts tasks")]
	public class StartTaskCommand: IManyTasksOptions
	{
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
		
		[Option('f', "force", HelpText = "Forces reboot, it task in already running")]
		public bool Force { get; set; }
	}
	
	[Verb("stop", HelpText = "Stops tasks")]
	public class StopTaskCommand: IManyTasksOptions
	{
		public IEnumerable<string> Names { get; set; } = Array.Empty<string>();
	}

}
