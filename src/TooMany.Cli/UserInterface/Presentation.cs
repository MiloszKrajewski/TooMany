using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using TooMany.Messages;

namespace TooMany.Cli.UserInterface
{
	public class Presentation
	{
		private static void NewLine() => AnsiConsole.WriteLine();

		private static void Write(ConsoleColor color, string? text)
		{
			AnsiConsole.Foreground = Color.FromConsoleColor(color);
			if (text != null) AnsiConsole.Write(text);
		}

		private static void WriteLine(ConsoleColor color, string? text)
		{
			Write(color, text);
			NewLine();
		}

		public static void Error(string text) => WriteLine(ConsoleColor.Red, text);

		public static void Warn(string text) => WriteLine(ConsoleColor.Yellow, text);

		public static void LogEvent(string task, LogEntryResponse message)
		{
			var color = message.Channel switch {
				LogChannel.StdErr => ConsoleColor.Red,
				LogChannel.StdOut => ConsoleColor.Cyan,
				_ => ConsoleColor.Gray
			};
			var time = message.Timestamp.ToLocalTime();
			var text = message.Text;

			Write(ConsoleColor.DarkGray, $"[{task} {time:s}] ");
			Write(color, text);
			NewLine();
		}

		public static void LogState(string task, TaskResponse? message)
		{
			var time = DateTime.Now;
			var state = message is null ? "[fuchsia]Removed[/]" : StateToAnsi(message);

			Write(ConsoleColor.Gray, $"[{task} {time:s}] ");
			Write(ConsoleColor.White, task);
			Write(ConsoleColor.Gray, " is ");
			AnsiConsole.Markup(state);
			NewLine();
		}

		public static void TaskInfo(IEnumerable<TaskResponse> tasks)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Name[/]");
			table.AddColumn("[white]Executable[/]");
			table.AddColumn("[white]Arguments[/]");
			table.AddColumn("[white]Directory[/]");
			table.AddColumn("[white]State[/]");
			table.AddColumn("[white]Uptime[/]");
			table.AddColumn("[white]Tags[/]");

			foreach (var task in tasks.OrderBy(t => t.Name))
			{
				TaskInfo(table, task);
			}

			AnsiConsole.Render(table);
		}

		private static void TaskInfo(Table table, TaskResponse task)
		{
			table.AddRow(
				$"[yellow]{task.Name.EscapeMarkup()}[/]",
				$"[white]{task.Executable.EscapeMarkup()}[/]",
				$"[white]{task.Arguments.EscapeMarkup()}[/]",
				$"[white]{task.Directory.EscapeMarkup()}[/]",
				StateToAnsi(task),
				$"[white]{RunningTime(task)}[/]",
				$"[white]{task.Tags.Join(",").EscapeMarkup()}[/]"
			);
		}

		public static void TaskDetails(IEnumerable<TaskResponse> tasks)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Property[/]");
			table.AddColumn("[white]Value[/]");

			foreach (var (index, task) in tasks.OrderBy(t => t.Name).WithIndex())
			{
				if (index > 0) table.AddEmptyRow();
				TaskDetails(table, task);
			}

			AnsiConsole.Render(table);
		}

		public static void TaskDetails(TaskResponse task)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Property[/]");
			table.AddColumn("[white]Value[/]");

			TaskDetails(table, task);

			AnsiConsole.Render(table);
		}

		private static void TaskDetails(Table table, TaskResponse task)
		{
			table.AddRow("[olive]Name[/]", $"[yellow]{task.Name.EscapeMarkup()}[/]");
			table.AddRow("[silver]Executable[/]", $"[white]{task.Executable.EscapeMarkup()}[/]");
			table.AddRow("[silver]Arguments[/]", $"[white]{task.Arguments.EscapeMarkup()}[/]");
			table.AddRow("[silver]Directory[/]", $"[white]{task.Directory.EscapeMarkup()}[/]");
			table.AddRow("[silver]State[/]", StateToAnsi(task));

			if (task.StartedTime.HasValue && task.ActualState == TaskState.Started)
			{
				var uptime = DateTime.UtcNow.Subtract(task.StartedTime.Value);
				table.AddRow("[silver]Uptime[/]", $"[white]{RunningTime(uptime)}[/]");
			}

			if (task.Environment.Count > 0)
			{
				var environmentTable = EnvironmentInfo(task.Environment);
				table.AddRow(new Markup("[silver]Variables[/]"), environmentTable);
			}

			if (task.Tags.Count > 0)
			{
				var tags = task.Tags.Join(",");
				table.AddRow("[silver]Tags[/]", $"[white]{tags.EscapeMarkup()}[/]");
			}
		}

		private static Table EnvironmentInfo(Dictionary<string, string?> environment)
		{
			var table = new Table { Border = TableBorder.None };
			table.AddColumn("Name");
			table.AddColumn("Value");
			table.HideHeaders();

			foreach (var kv in environment.OrderBy(kv => kv.Key))
			{
				var name = kv.Key.EscapeMarkup();
				var value = kv.Value?.EscapeMarkup();
				table.AddRow(
					$"[silver]{name}[/]",
					value is null ? "[red](delete)[/]" :
					string.IsNullOrWhiteSpace(value) ? "[grey](empty)[/]" :
					$"[white]{value}[/]");
			}

			return table;
		}

		private static string StateToAnsi(TaskResponse task)
		{
			if (task.ExpectedState == task.ActualState)
				return StateToAnsi(task.ActualState);

			return (task.ExpectedState, task.ActualState) switch {
				(TaskState.Stopped, TaskState.Started) => "[yellow]Stopping[/]",
				(TaskState.Started, TaskState.Stopped) => "[yellow]Starting[/]",
				(TaskState.Started, TaskState.Failed) => "[yellow]Restarting[/]",
				_ => StateToAnsi(task.ActualState),
			};
		}

		private static string StateToAnsi(TaskState state) =>
			state switch {
				TaskState.Started => "[green]Running[/]",
				TaskState.Stopped => "[grey]Stopped[/]",
				TaskState.Failed => "[red]Failed[/]",
				_ => "[yellow]Unknown[/]"
			};

		public static string RunningTime(TaskResponse task) =>
			task.StartedTime is null || task.ActualState != TaskState.Started
				? string.Empty
				: RunningTime(DateTime.UtcNow.Subtract(task.StartedTime.Value));

		public static string RunningTime(TimeSpan time) =>
			time.TotalDays > 999 ? $"{time.TotalDays:0}d" :
			time.TotalDays > 1 ? $"{time.TotalDays:0}d {time.Hours:0}h" :
			time.TotalHours > 1 ? $"{time.TotalHours:0}h {time.Minutes:0}m" :
			time.TotalMinutes > 1 ? $"{time.TotalMinutes:0}m {time.Seconds:0}s" :
			$"{time.TotalSeconds:0}s";
	}
}
