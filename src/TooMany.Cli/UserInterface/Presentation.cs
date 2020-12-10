using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using TooMany.Messages;

namespace TooMany.Cli.UserInterface
{
	public class Presentation
	{
		public static void TaskInfo(IEnumerable<TaskResponse> tasks)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Name[/]");
			table.AddColumn("[white]Executable[/]");
			table.AddColumn("[white]Arguments[/]");
			table.AddColumn("[white]Directory[/]");
			table.AddColumn("[white]State[/]");
			table.AddColumn("[white]Uptime[/]");

			foreach (var task in tasks)
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
				$"[white]{RunningTime(task)}[/]"
			);
		}

		public static void TaskDetails(IEnumerable<TaskResponse> tasks)
		{
			var table = new Table { Border = TableBorder.Simple };

			table.AddColumn("[silver]Property[/]");
			table.AddColumn("[white]Value[/]");

			foreach (var (index, task) in tasks.WithIndex())
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
