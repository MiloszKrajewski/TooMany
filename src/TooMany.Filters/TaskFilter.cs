using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using K4os.BoolEx;
using K4os.BoolEx.Parsing;
using K4os.Shared;

namespace TooMany.Filters
{
	public class TaskFilter
	{
		private class TagParser: ExpressionParser
		{
			// It is similar but not the same to ValidTagRegex 
			public override Regex IdentRegex => new("#?(\\w|\\d|_|\\*|\\?)+");
		}

		private static readonly ExpressionParser Parser = new TagParser();

		private readonly ICacheAdapter _cache;
		private readonly Func<IEnumerable<string>, bool> _isMatch;
		private readonly bool _isValid;

		public TaskFilter(
			string? filterExpression, ICacheAdapter? cache = null)
		{
			_cache = cache ?? new DumbCache();
			var matcher = ExpressionMatcher(filterExpression, true); 
			_isValid = matcher is not null;
			_isMatch = matcher ?? AlwaysFalse;
		}
		
		private static bool AlwaysFalse(IEnumerable<string> _) => false;

		// private static readonly TimeSpan MatcherExpiration = TimeSpan.FromMinutes(1);

		// private static readonly MemoryCacheOptions CacheOptions = new() {
		// 	SizeLimit = 1000,
		// 	ExpirationScanFrequency = TimeSpan.FromMinutes(5),
		// };
		//
		// private static readonly IMemoryCache Cache = new MemoryCache(CacheOptions);

		// private static Func<string, bool> GetMatcher(string expected, bool ignoreCase) =>
		// 	Cache.GetOrCreate(
		// 		(expected, ignoreCase), static e => {
		// 			e.Size = 1;
		// 			e.SlidingExpiration = MatcherExpiration;
		// 			// extract from key to allow static lambda - performance
		// 			var (p, ic) = (ValueTuple<string, bool>) e.Key;
		// 			return CreateMatcher(p, ic);
		// 		});

		public bool IsValid => _isValid;

		public bool IsMatch(string? name, IEnumerable<string>? tags = null) =>
			name is not null &&
			_isMatch(tags.NotNull().Select(t => $"#{t}").Prepend(name).ToArray());

		private Func<IEnumerable<string>, bool>? ExpressionMatcher(
			string? filterExpression, bool ignoreCase = false)
		{
			var expression = TryCompile(filterExpression);
			if (expression is null) return null;

			return tags => expression.Evaluate(t => AnyMatch(t as string, tags, ignoreCase));
		}

		private bool AnyMatch(
			string? expected, IEnumerable<string> actual, bool ignoreCase) =>
			expected is not null && actual.Any(GetPatternMatcher(expected, ignoreCase));

		private Func<string, bool> GetPatternMatcher(string pattern, bool ignoreCase) =>
			_cache.GetOrCreate(
				(pattern, ignoreCase),
				kv => CreatePatternMatcher(kv.pattern, kv.ignoreCase))!;

		private static Func<string, bool> CreatePatternMatcher(string pattern, bool ignoreCase) =>
			Wildcard.Matcher(pattern, ignoreCase);

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
	}
}
