using System;
using System.Collections.Generic;
using K4os.BoolEx;
using System.Linq;
using System.Text.RegularExpressions;
using K4os.BoolEx.Parsing;
using K4os.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace TooMany.Actors.Filter
{
	public class FilterExpression
	{
		public class TagParser: ExpressionParser
		{
			// It is similar but not the same to ValidTagRegex 
			public override Regex IdentRegex => new("#?(\\w|\\d|_|\\*|\\?)+");
		}

		private static readonly ExpressionParser Parser = new TagParser();

		private static readonly TimeSpan MatcherExpiration = TimeSpan.FromMinutes(1);

		private static readonly MemoryCacheOptions CacheOptions = new() {
			SizeLimit = 1000,
			ExpirationScanFrequency = TimeSpan.FromMinutes(5),
		};

		private static readonly IMemoryCache Cache = new MemoryCache(CacheOptions);

		public static Func<IEnumerable<string>, bool>? Matcher(
			string? filterExpression, bool ignoreCase = false)
		{
			var expression = TryCompile(filterExpression);
			if (expression is null) return null;

			return tags => expression.Evaluate(t => AnyMatch(t as string, tags, ignoreCase));
		}

		private static Expression? TryCompile(string? tags)
		{
			if (string.IsNullOrWhiteSpace(tags))
				return Expression.True;

			try
			{
				return Parser.FromString(tags);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static bool AnyMatch(
			string? expected, IEnumerable<string> actual, bool ignoreCase) =>
			expected is not null && actual.Any(GetMatcher(expected, ignoreCase));

		private static Func<string, bool> GetMatcher(string expected, bool ignoreCase) =>
			Cache.GetOrCreate(
				(expected, ignoreCase), static e => {
					e.Size = 1;
					e.SlidingExpiration = MatcherExpiration;
					// extract from key to allow static lambda - performance
					var (p, ic) = (ValueTuple<string, bool>) e.Key;
					return Wildcard.Matcher(p, ic);
				});
	}
}
