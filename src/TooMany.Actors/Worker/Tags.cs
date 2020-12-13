using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TooMany.Actors.Worker
{
	public class Tags
	{
		private static readonly Regex ValidTagPattern = new Regex(@"(\w|\d|_)+");

		private static bool IsValidTag(string tag) => ValidTagPattern.IsMatch(tag);

		public static string[]? InvalidTags(IEnumerable<string> tags) => 
			tags.Where(t => !IsValidTag(t)).ToArray().NullIfEmpty();
	}
}
