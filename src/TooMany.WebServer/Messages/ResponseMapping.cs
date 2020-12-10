using System;
using System.Linq;
using TooMany.Actors.Messages;
using TooMany.Actors.Worker;
using TooMany.Messages;

namespace TooMany.WebServer.Messages
{
	public static class ResponseMapping
	{
		public static DefineTask ToCommand(this TaskRequest task, string name) =>
			new DefineTask {
				Name = name,
				Executable = task.Executable ?? throw new ArgumentException("Executable is empty"),
				Arguments = task.Arguments ?? string.Empty,
				Directory = task.Directory ?? string.Empty,
				Environment = task.Environment.NotNull().ToDictionary()
			};

		public static TaskResponse ToResponse(this TaskSnapshot snapshot)
		{
			return new TaskResponse {
				Name = snapshot.Name,
				Executable = snapshot.Executable,
				Arguments = snapshot.Arguments,
				Directory = snapshot.Directory,
				ExpectedState = snapshot.ExpectedState,
				ActualState = snapshot.ActualState,
				StartedTime = snapshot.StartedTime,
				Environment = snapshot.Environment.ToDictionary()
			};
		}
		
		private static LogEntryResponse ToResponse(LogEntry entry) =>
			new LogEntryResponse {
				Channel = entry.Error ? LogChannel.StdErr : LogChannel.StdOut,
				Timestamp = entry.Timestamp,
				Text = entry.Text
			};

		public static TaskResponse[] ToResponse(this ManyTasksSnapshot snapshots) =>
			snapshots.Tasks.Select(ToResponse).ToArray();

		public static LogEntryResponse[] ToResponse(this TaskLog log) =>
			log.Messages.Select(ToResponse).ToArray();
	}
}
