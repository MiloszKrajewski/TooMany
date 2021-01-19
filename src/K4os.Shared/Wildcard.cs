using System;
using System.Text.RegularExpressions;

namespace K4os.Shared
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

		public static bool IsWildcard(string? pattern)
		{
			if (string.IsNullOrWhiteSpace(pattern)) 
				return false;

			var length = pattern!.Length;
			for (var i = 0; i < length; i++)
			{
				var isGlob = pattern[i] switch { '*' or '?' => true, _ => false };
				if (isGlob) return true;
			}

			return false;
		}

		public static Func<string?, bool> Matcher(string? pattern, bool ignoreCase)
		{
			if (pattern is null)
				return NullMatcher;
			
			if (pattern == "*")
				return AllMatcher;
			
			if (IsWildcard(pattern))
			{
				var wildcard = new Wildcard(pattern, ignoreCase);
				return s => s != null && wildcard.IsMatch(s);
			}

			var comparison = ignoreCase
				? StringComparison.InvariantCultureIgnoreCase
				: StringComparison.InvariantCulture;

			return s => string.Equals(s, pattern, comparison);
		}
		
		private static bool NullMatcher(string? text) => text is null;
		
		private static bool AllMatcher(string? _) => true;
	}
}
