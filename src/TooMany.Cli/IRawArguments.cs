using System;

namespace TooMany.Cli
{
	public interface IRawArguments
	{
		string[] Head { get; }
		string[] Tail { get; }
	}

	internal class RawArguments: IRawArguments
	{
		public string[] Head { get; }
		public string[] Tail { get; }

		public RawArguments(string[] args) =>
			(Head, Tail) = Array.FindIndex(args, s => s == "--") switch {
				< 0 => (args, Array.Empty<string>()),
				var p => (args[..p], args[(p + 1)..]),
			};
	}
}
