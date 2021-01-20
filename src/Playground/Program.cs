using System;
using System.Linq;
using Spectre.Console.Cli;

namespace Playground
{
	class Program
	{
		static void Main(string[] args)
		{
			var a = new[] { 0, 1 };
			Console.WriteLine(a[0]);
			var b = a[1..];
			Console.WriteLine(b.Length);
		}

		private static void RunArgs(string[] args)
		{
			var app = new CommandApp();
			app.Configure(config => config.AddCommand<RunCommand>("run"));
			app.Run(args);
		}
	}
}
