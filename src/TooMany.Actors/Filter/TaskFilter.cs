using System;
using System.Collections.Generic;
using System.Linq;
using TooMany.Actors.Messages;

namespace TooMany.Actors.Filter
{
	public class TaskFilter
	{
		private readonly Func<IEnumerable<string>, bool> _isMatch;

		public TaskFilter(string? expression)
		{
			_isMatch =
				FilterExpression.Matcher(expression, true) ??
				throw new ArgumentException($"Expression '{expression}' could not be parsed");
		}

		public bool IsMatch(TaskSnapshot? task)
		{
			if (task is null) return false;

			var name = task.Name;
			var tags = task.Tags.NotNull().Select(t => $"#{t}").Prepend(name).ToArray();
			return _isMatch(tags);
		}
	}
}
