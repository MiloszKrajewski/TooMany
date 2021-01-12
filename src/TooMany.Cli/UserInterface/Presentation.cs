using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using TooMany.Messages;

namespace TooMany.Cli.UserInterface
{
	public static class Presentation
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
			var state = message is null ? Markup("fuchsia", "Removed") : StateToAnsi(message);

			Write(ConsoleColor.Gray, $"[{task} {time:s}] ");
			Write(ConsoleColor.White, task);
			Write(ConsoleColor.Gray, " is ");
			AnsiConsole.Render(state);
			NewLine();
		}

		public static void TaskInfo(IEnumerable<TaskResponse> tasks)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Name[/]");
			table.AddColumn("[white]Tags[/]");
			table.AddColumn("[white]State[/]");
			table.AddColumn("[white]Uptime[/]");
			table.AddColumn("[white]Executable[/]");
			table.AddColumn("[white]Arguments[/]");
			table.AddColumn("[white]Directory[/]");

			foreach (var task in tasks.OrderBy(t => t.Name))
			{
				TaskInfo(table, task);
			}

			AnsiConsole.Render(table);
		}

		private static void TaskInfo(Table table, TaskResponse task)
		{
			var (executable, arguments) = FixExecutableAndArguments(task, 30, 40);
			var directory = task.Directory.NotNull().Trim().TruncatePath(30);

			static Markup Yellow(string value) => Markup("yellow", value);
			static Markup White(string value) => Markup("white", value);

			table.AddRow(
				Yellow(task.Name),
				White(TagsCsv(task)),
				StateToAnsi(task),
				White(RunningTime(task)),
				executable,
				arguments,
				White(directory)
			);
		}

		private static string TagsCsv(TaskResponse task) =>
			task.Tags.OrderBy(x => x).Select(t => $"#{t}").Join(",");

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
			var (executable, arguments) = FixExecutableAndArguments(task, 128, 1024);
			static Markup Silver(string text) => Markup("silver", text);

			table.AddRow(Markup("olive", "Name"), Markup("yellow", task.Name));
			table.AddRow(Silver("Executable"), executable);
			table.AddRow(Silver("Arguments"), arguments);
			table.AddRow(Silver("Directory"), Markup("white", task.Directory));

			if (task.Environment.Count > 0)
			{
				var environmentTable = EnvironmentInfo(task.Environment);
				table.AddRow(Markup("silver", "Variables"), environmentTable);
			}

			if (task.Tags.Count > 0)
			{
				var tags = TagsCsv(task);
				table.AddRow(Markup("silver", "Tags"), Markup("white", tags));
			}

			table.AddRow(Silver("State"), StateToAnsi(task));

			if (task.StartedTime.HasValue && task.ActualState == TaskState.Started)
			{
				var uptime = DateTime.UtcNow.Subtract(task.StartedTime.Value);
				table.AddRow(Markup("silver", "Uptime"), Markup("white", RunningTime(uptime)));
			}
		}

		public static void TaskSpecs(IEnumerable<TaskResponse> tasks)
		{
			foreach (var task in tasks) TaskSpec(task);
		}

		private static void TaskSpec(TaskResponse task)
		{
			var space = new Text(" ");

			// 2many define <name> -s -t <tags...> -d <folder> <executable> -- <arguments...>
			IEnumerable<Markup> Compose()
			{
				yield return Markup("grey", "2many define");
				yield return Markup("yellow", task.Name.Quote());

				if (task.UseShell)
					yield return Markup("grey", "-s");

				var tags = task.Tags.Join(",");
				if (!string.IsNullOrWhiteSpace(tags))
				{
					yield return Markup("grey", "-t");
					yield return Markup("white", tags);
				}

				if (!string.IsNullOrWhiteSpace(task.Directory))
				{
					yield return Markup("grey", "-d");
					yield return Markup("white", task.Directory.Quote());
				}

				yield return Markup("yellow", task.Executable.Quote());

				if (!string.IsNullOrWhiteSpace(task.Arguments))
				{
					yield return Markup("grey", "--");
					yield return Markup("yellow", task.Arguments);
				}
			}

			foreach (var (i, m) in Compose().WithIndex())
			{
				if (i > 0) AnsiConsole.Render(space);
				AnsiConsole.Render(m);
			}

			NewLine();
		}

		private static (Markup Executable, Markup Arguments) FixExecutableAndArguments(
			TaskResponse task, int executableWidth, int argumentWidth)
		{
			var shell = task.UseShell;
			var executable = shell
				? Markup("green", "(shell)")
				: Markup("white", task.Executable.TruncatePath(executableWidth));
			var arguments = (shell
					? $"{task.Executable.Quote()} {task.Arguments}"
					: task.Arguments
				).PipeTo(x => Markup("white", x.TruncateString(argumentWidth)));
			return (executable, arguments);
		}

		private static Markup Markup(string color, string value) =>
			new Markup($"[{color}]{value.Trim().EscapeMarkup()}[/]");

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

		private static Markup StateToAnsi(TaskResponse task)
		{
			if (task.ExpectedState == task.ActualState)
				return StateToAnsi(task.ActualState);

			static Markup Yellow(string value) => Markup("yellow", value);

			return (task.ExpectedState, task.ActualState) switch {
				(TaskState.Stopped, TaskState.Started) => Yellow("Stopping"),
				(TaskState.Started, TaskState.Stopped) => Yellow("Starting"),
				(TaskState.Started, TaskState.Failed) => Yellow("Restarting"),
				_ => StateToAnsi(task.ActualState),
			};
		}

		private static Markup StateToAnsi(TaskState state) =>
			state switch {
				TaskState.Started => Markup("green", "Running"),
				TaskState.Stopped => Markup("grey", "Stopped"),
				TaskState.Failed => Markup("red", "Failed"),
				_ => Markup("yellow", "Unknown")
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

		public static Task WaitWith(this Task task, string text) =>
			AnsiConsole.Status().Spinner(Spinner.Known.Default).StartAsync(text, _ => task);

		public static async Task<T> WaitWith<T>(this Task<T> task, string text)
		{
			var result = default(T);
			await AnsiConsole
				.Status()
				.Spinner(Spinner.Known.Default)
				.StartAsync(text, async _ => result = await task);
			return result!;
		}
	}
}
