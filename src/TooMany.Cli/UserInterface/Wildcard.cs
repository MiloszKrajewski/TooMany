using System;
using System.Text.RegularExpressions;

namespace TooMany.Cli.UserInterface
{
	public class Wildcard
	{
		private readonly Regex _rx;

		public Wildcard(string pattern, bool ignoreCase = false)
		{
			_rx = new Regex(
				"^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$",
				ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		}

		public bool IsMatch(string text) => _rx.IsMatch(text);

		public static bool IsWildcard(string pattern) =>
			pattern.Contains('*') || pattern.Contains('?');

		public static Func<string, bool> Matcher(string pattern, bool ignoreCase = false)
		{
			if (IsWildcard(pattern))
			{
				var wildcard = new Wildcard(pattern, ignoreCase);
				return s => wildcard.IsMatch(s);
			}

			if (ignoreCase)
			{
				return s => string.Equals(s, pattern, StringComparison.InvariantCultureIgnoreCase);
			}

			return s => string.Equals(s, pattern, StringComparison.InvariantCulture);
		}
	}
}
