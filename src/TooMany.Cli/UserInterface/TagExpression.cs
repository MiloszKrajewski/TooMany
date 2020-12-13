using System;
using System.Collections.Generic;
using System.Linq;
using K4os.BoolEx;
using K4os.BoolEx.Parsing;

namespace TooMany.Cli.UserInterface
{
	public class TagExpression
	{
		public static Func<IEnumerable<string>, bool>? Matcher(
			string? tagExpression, bool ignoreCase = false)
		{
			var expression = TryCompile(tagExpression);
			if (expression is null) return null;

			return tags => expression.Evaluate(t => AnyMatch(t as string, tags, ignoreCase));
		}

		private static Expression? TryCompile(string? tags)
		{
			if (string.IsNullOrWhiteSpace(tags))
				return Expression.True;

			try
			{
				return ExpressionParser.Default.FromString(tags);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static bool AnyMatch(string? expected, IEnumerable<string> actual, bool ignoreCase)
		{
			if (expected is null)
				return false;

			var comparer = ignoreCase
				? StringComparison.InvariantCultureIgnoreCase
				: StringComparison.InvariantCulture;

			bool IsMatch(string tag) => string.Equals(tag, expected, comparer);

			return actual.Any(IsMatch);
		}
	}
}
