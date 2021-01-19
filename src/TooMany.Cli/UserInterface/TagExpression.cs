using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TooMany.Cli.UserInterface
{
	public class TagExpression
	{
		private static readonly Regex ValidTagRegex = new("^#?(?<name>(\\w|\\d|_)+)$");
		
		public static string[] ExpandTags(IEnumerable<string>? tags) =>
			tags
				.NotNull()
				.SelectMany(t => t.Split(','))
				.Select(t => t.Trim())
				.Select(ToTag)
				.ToArray();

		private static string ToTag(string tag)
		{
			var m = ValidTagRegex.Match(tag);
			return !m.Success 
				? throw new ArgumentException($"'{tag}' does not look like valid tag") 
				: m.Groups["name"].Value;
		}
	}
}
