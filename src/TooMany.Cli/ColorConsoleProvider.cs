using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TooMany.Cli
{
	internal class ColorConsoleProvider: ILoggerProvider
	{
		public ILogger Default = new ColorConsoleLogger(true);
		
		private readonly ConcurrentDictionary<string, ILogger> _loggers =
			new ConcurrentDictionary<string, ILogger>();

		private readonly string[] _prefixes;
		
		public ColorConsoleProvider(params string[] prefixes) { _prefixes = prefixes; }

		public ILogger CreateLogger(string categoryName) => GetLogger(categoryName);

		private ILogger GetLogger(string categoryName) =>
			_loggers.GetOrAdd(categoryName, NewLogger);

		private ILogger NewLogger(string categoryName) =>
			string.IsNullOrWhiteSpace(categoryName)
				? (ILogger) NullLogger.Instance
				: new ColorConsoleLogger(_prefixes.Any(categoryName.StartsWith));

		public void Dispose() { }
	}

	internal class ColorConsoleLogger: ILogger
	{
		private readonly bool _verbose;

		public ColorConsoleLogger(bool verbose) { _verbose = verbose; }

		public void Log<TState>(
			LogLevel logLevel, EventId eventId, TState state, Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			try
			{
				Log(logLevel, formatter(state, exception));
				if (exception is null) return;

				Log(logLevel, $"{exception.GetType().Name}: {exception.Message}");
				if (string.IsNullOrWhiteSpace(exception.StackTrace)) return;

				Log(logLevel, exception.StackTrace);
			}
			catch (Exception e)
			{
				Log(LogLevel.Warning, $"<internal logging error: {e.GetType().Name}>");
			}
		}

		private static void Log(LogLevel logLevel, string? message)
		{
			if (string.IsNullOrWhiteSpace(message))
				return;

			lock (Console.Out)
			{
				var color = Console.ForegroundColor;
				Console.ForegroundColor = ToColor(logLevel);
				Console.WriteLine(message);
				Console.ForegroundColor = color;
			}
		}

		private static ConsoleColor ToColor(LogLevel logLevel) =>
			logLevel switch {
				LogLevel.Debug => ConsoleColor.Gray,
				LogLevel.Information => ConsoleColor.Cyan,
				LogLevel.Warning => ConsoleColor.Yellow,
				LogLevel.Error => ConsoleColor.Red,
				LogLevel.Critical => ConsoleColor.Magenta,
				_ => ConsoleColor.DarkGray
			};

		public bool IsEnabled(LogLevel logLevel) =>
			_verbose || logLevel >= LogLevel.Warning;

		public IDisposable? BeginScope<TState>(TState state) => null;
	}
}
