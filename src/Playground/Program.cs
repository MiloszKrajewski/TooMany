using System;
using System.Linq;
using Spectre.Console.Cli;

namespace Playground
{
	class Program
	{
		static void Main(string[] args)
		{
			var app = new CommandApp();
			app.Configure(config => config.AddCommand<RunCommand>("run"));
			app.Run(args);
		}
	}
}
