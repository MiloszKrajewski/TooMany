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
		public static Regex ValidTagRegex = new("^#?(?<name>(\\w|\\d|_)+)$");
		
		public static string[] ExpandTags(IEnumerable<string>? tags) =>
			tags
				.NotNull()
				.SelectMany(t => t.Split(','))
				.Select(t => t.Trim())
				.Select(ToTag)
				.ToArray();

		private static string ToTag(string tag)
		{
			var m = TagExpression.ValidTagRegex.Match(tag);
			return !m.Success 
				? throw new ArgumentException($"'{tag}' does not look like valid tag") 
				: m.Groups["name"].Value;
		}
		
		public class TagParser: ExpressionParser
		{
			// It is similar but not the same to ValidTagRegex 
			public override Regex IdentRegex => new("#?(\\w|\\d|_|\\*|\\?)+");
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

			bool IsMatch(string tag) => Wildcard.Matcher(expected, ignoreCase, true)(tag);

			return actual.Any(IsMatch);
		}
	}
}
