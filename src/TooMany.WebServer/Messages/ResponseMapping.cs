using System;
using System.Collections.Immutable;
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
				Environment = task.Environment.EmptyIfNull().ToImmutableDictionary(),
				Tags = task.Tags.EmptyIfNull().NoNulls().ToImmutableArray()
			};
		
		public static SetTags ToCommand(this TagsRequest tags, string name) =>
			new SetTags {
				Name = name,
				Tags = tags.Tags.EmptyIfNull().NoNulls().ToImmutableArray()
			};

		public static TaskResponse ToResponse(this TaskSnapshot snapshot) =>
			new TaskResponse {
				Name = snapshot.Name,
				Executable = snapshot.Executable,
				Arguments = snapshot.Arguments,
				Directory = snapshot.Directory,
				ExpectedState = snapshot.ExpectedState,
				ActualState = snapshot.ActualState,
				StartedTime = snapshot.StartedTime,
				Environment = snapshot.Environment.ToDictionary(),
				Tags = snapshot.Tags.ToList()
			};

		public static LogEntryResponse ToResponse(this LogEntry entry) =>
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
