using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace Playground
{
	public class RunCommand: AsyncCommand<RunCommand.Settings>
	{
		public class Settings: CommandSettings { }

		public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
		{
			await Task.CompletedTask;
			return 0;
		}
		
		public static T Measure<T>(string name, Func<T> func)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				return func();
			}
			finally
			{
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine("{0}: {1:0.00}ms", name, elapsed);
			}
		}
		
		public static void Measure(string name, Action func)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				func();
			}
			finally
			{
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine("{0}: {1:0.00}ms", name, elapsed);
			}
		}
		
		public static async Task<T> Measure<T>(string name, Func<Task<T>> func)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				return await func();
			}
			finally
			{
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine("{0}: {1:0.00}ms", name, elapsed);
			}
		}

		public static async Task Measure(string name, Func<Task> func)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				await func();
			}
			finally
			{
				var elapsed = stopwatch.Elapsed.TotalMilliseconds;
				Console.WriteLine("{0}: {1:0.00}ms", name, elapsed);
			}
		}
	}

	public class MeasureBlock: IDisposable
	{
		private readonly string _name;
		private readonly Stopwatch _watch;

		public MeasureBlock(string name)
		{
			_name = name;
			_watch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			_watch.Stop();
			var elapsed = _watch.Elapsed.TotalMilliseconds;
			Console.WriteLine("{0}: {1:0.00}ms", _name, elapsed);
		}
	}
}
