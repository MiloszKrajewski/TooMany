using System;
using System.Collections.Concurrent;
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

		public static Func<string?, bool> Matcher(
			string? pattern, bool ignoreCase, bool useCache = false)
		{
			if (pattern is null)
				return NullMatcher;
			
			if (useCache)
				return GetMatcher(pattern, ignoreCase);

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

		private static readonly ConcurrentDictionary<(string, bool), Func<string?, bool>> Matchers
			= new();
		
		private static bool NullMatcher(string? text) => text is null;
		
		private static Func<string?, bool> GetMatcher(string pattern, bool ignoreCase) => 
			Matchers.GetOrAdd((pattern, ignoreCase), NewMatcher);

		private static Func<string?, bool> NewMatcher((string Pattern, bool IgnoreCase) filter) =>
			Matcher(filter.Pattern, filter.IgnoreCase);

	}
}
