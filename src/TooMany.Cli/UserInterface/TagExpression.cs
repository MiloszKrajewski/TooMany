using System;
using System.Collections.Generic;
using K4os.BoolEx;
using System.Linq;
using System.Text.RegularExpressions;
using K4os.BoolEx.Parsing;

namespace TooMany.Cli.UserInterface
{
	public class TagExpression
	{
		public class TagParser: ExpressionParser
		{
			public override Regex IdentRegex => new Regex("#?(\\w|\\d|_|\\*|\\?)+");
		}

		private static readonly ExpressionParser Parser = new TagParser();

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
				return Parser.FromString(tags);
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static bool AnyMatch(string? expected, IEnumerable<string> actual, bool ignoreCase)
		{
			if (expected is null) return false;

			bool IsMatch(string tag) => Wildcard.Matcher(tag, ignoreCase, true)(expected);

			return actual.Any(IsMatch);
		}
	}
}
