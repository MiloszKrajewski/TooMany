using System;
using System.Collections.Generic;
using System.Linq;
using TooMany.Actors.Messages;

namespace TooMany.Actors.Filter
{
	public class TaskFilter
	{
		private readonly Func<IEnumerable<string>, bool> _isMatch;

		public static TaskFilter? TryCreate(string? expression) =>
			TryCompile(expression)?.PipeTo(e => new TaskFilter(e));

		public TaskFilter(string? expression): this(Compile(expression)) { }
		
		protected TaskFilter(Func<IEnumerable<string>, bool> isMatch)
		{
			_isMatch = isMatch;
		}

		public bool IsMatch(TaskSnapshot? task)
		{
			if (task is null) return false;

			var name = task.Name;
			var tags = task.Tags.NotNull().Select(t => $"#{t}").Prepend(name).ToArray();
			return _isMatch(tags);
		}
		
		private static Func<IEnumerable<string>, bool> Compile(string? expression) =>
			TryCompile(expression) ??
			throw new ArgumentException($"Expression '{expression}' could not be parsed");

		private static Func<IEnumerable<string>, bool>? TryCompile(string? expression) => 
			FilterExpression.Matcher(expression, true);
	}
}
