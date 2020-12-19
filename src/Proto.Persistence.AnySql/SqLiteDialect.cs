namespace Proto.Persistence.AnySql
{
	/// <summary>
	/// Reference implementation of <see cref="IAnySqlDialect"/> for MySql.
	/// </summary>
	/// <seealso cref="IAnySqlDialect" />
	public class SqLiteDialect: IAnySqlDialect
	{
		private static string Combine(string? schemaName, string tableName) =>
			string.IsNullOrWhiteSpace(schemaName)
				? $"\"{tableName}\""
				: $"\"{schemaName}_{tableName}\"";

		public string CreateSchema(string schemaName) => "pragma _";

		public string EventsTable(string? schemaName, string tableName) =>
			Combine(schemaName, tableName + "_events");

		public string SnapshotsTable(string? schemaName, string tableName) =>
			Combine(schemaName, tableName + "_snapshots");

		private static string CreateEither(string objectName) =>
			$@"
				create table if not exists {objectName} (
					""actor"" text not null,
					""index"" integer not null,
					""data"" text,
					primary key (`actor`, `index`)
				)
			";

		private static string InsertEither(string objectName, string keyword = "insert") =>
			$@"
				{keyword} into {objectName} (""actor"", ""index"", ""data"") 
				values (@actor, @index, @data)
			";

		private static string DeleteEither(string objectName) =>
			$@"
				delete from {objectName} 
				where ""actor"" = @actor and ""index"" <= @index_end
			";

		public string SelectEvents(string objectName) =>
			$@"
				select ""index"", ""data"" 
				from {objectName}
				where ""actor"" = @actor and ""index"" between @index_start and @index_end
				order by ""index"" asc
			";

		public string SelectSnapshot(string objectName) =>
			$@"
				select ""index"", ""data"" 
				from {objectName}
				where ""actor"" = @actor
				order by ""index"" desc
				limit 1
			";

		public string CreateEventsTable(string objectName) => CreateEither(objectName);

		public string CreateSnapshotsTable(string objectName) => CreateEither(objectName);

		public string InsertEvent(string objectName) => InsertEither(objectName);

		public string DeleteEvents(string objectName) => DeleteEither(objectName);

		public string InsertSnapshot(string objectName) => InsertEither(objectName, "replace");

		public string DeleteSnapshots(string objectName) => DeleteEither(objectName);
	}
}
