using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HttpRemoting.Data;
using HttpRemoting.Server;
using K4os.Json.KnownTypes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Proto;
using Proto.Persistence;
using Proto.Persistence.AnySql;
using Serilog;
using Serilog.Events;
using TooMany.Actors;
using TooMany.Actors.Catalog;
using TooMany.Actors.Tools;
using TooMany.Actors.Worker;
using TooMany.Host.Frontend;
using TooMany.WebServer;
using Log = Serilog.Log;
using SystemProcess = System.Diagnostics.Process;

namespace TooMany.Host
{
	internal static class Program
	{
		private static readonly Type ThisType = typeof(Program);

		private static readonly string AssemblyPath =
			Path.GetDirectoryName(SystemProcess.GetCurrentProcess().MainModule.FileName)!;

		private static readonly CancellationTokenSource Cancel = new CancellationTokenSource();

		[STAThread]
		private static async Task Main()
		{
			try
			{
				var host = new HostBuilder()
					.UseContentRoot(AssemblyPath)
					.ConfigureHostConfiguration(ConfigureHost)
					.ConfigureAppConfiguration(ConfigureApp)
					.ConfigureLogging(ConfigureLogging)
					.UseServiceProviderFactory(ConfigureContainer)
					.ConfigureWebHostDefaults(ConfigureWebServer)
					.ConfigureServices(ConfigureServices)
					.Build();
				var token = Cancel.Token;
				await host.RunAsync(token);
			}
			catch (Exception e)
			{
				ReportException(e);
			}
		}

		private static void ConfigureHost(IConfigurationBuilder builder)
		{
			builder.AddCommandLine(Environment.GetCommandLineArgs());
			builder.AddEnvironmentVariables(@"TooMany_");
			builder.AddJsonFile("TooMany.json", true);
			builder.AddJsonFile($"{ThisType.Namespace}.json", true);
		}

		private static void ConfigureApp(
			HostBuilderContext context, IConfigurationBuilder builder) { }

		private static void ConfigureLogging(
			HostBuilderContext context, ILoggingBuilder builder)
		{
			const string outputTemplate =
				"{Timestamp:HH:mm:ss} [{Level:u4}] ({SourceContext:l}) {Message:lj}{NewLine}{Exception}";
			var rootPath = context.HostingEnvironment.ContentRootPath;
			var outputFilename = Path.Combine(rootPath, $"{ThisType.Namespace}.log");
			var logger = new LoggerConfiguration()
				.MinimumLevel.Warning()
				.MinimumLevel.Override(nameof(TooMany), LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.WriteTo.File(outputFilename, outputTemplate: outputTemplate)
				.CreateLogger();
			builder.AddSerilog(logger);
		}

		private static DefaultServiceProviderFactory ConfigureContainer(
			HostBuilderContext context) =>
			new DefaultServiceProviderFactory();

		private static void ConfigureServices(
			HostBuilderContext context, IServiceCollection services)
		{
			var connectionString = new SqliteConnectionStringBuilder {
				DataSource = Path.Combine(AssemblyPath, $"{ThisType.Namespace}.sqlite3"),
			}.ToString();

			services.AddSingleton(Cancel);
			services.AddHostedService<FrontendService>();
			services.AddHostedService<ActorsService>();

			services.AddSingleton<IHttpRemotingResponseBuilder, HttpRemotingResponseBuilder>();
			services.AddSingleton<IHttpRemotingErrorAdapter, DefaultHttpRemotingErrorAdapter>();

			var binder = KnownTypesSerializationBinder.Default;
			binder.RegisterAssembly<TaskCatalog>();

			services.AddSingleton(
				new JsonSerializerSettings {
					TypeNameHandling = TypeNameHandling.Auto,
					NullValueHandling = NullValueHandling.Ignore,
					SerializationBinder = binder,
				});
			services.AddTransient(p => CreateConnection(connectionString));
			services.AddSingleton<IAnySqlDialect>(new SqLiteDialect());
			services.AddSingleton(CreatePersistence);
			services.AddSingleton(typeof(ITypedProps<>), typeof(TypedProps<>));
			services.AddSingleton<IReceiverMiddleware, DebugLoggingMiddleware>();

			services.AddSingleton<ActorSystem>();
			services.AddSingleton<RootContext>();

			services.AddTransient<TaskCatalogActor>();
			services.AddTransient<TaskRunnerActor>();
		}

		private static IProvider CreatePersistence(IServiceProvider serviceProvider)
		{
			var settings = serviceProvider.GetRequiredService<JsonSerializerSettings>();
			var dialect = serviceProvider.GetRequiredService<IAnySqlDialect>();

			DbConnection Connect() =>
				serviceProvider.GetRequiredService<DbConnection>();

			string Serialize(object o) =>
				JsonConvert.SerializeObject(o, typeof(object), settings);

			object? Deserialize(string s) =>
				JsonConvert.DeserializeObject(s, typeof(object), settings);

			return new AnySqlProvider(
				Connect, null, "TooMany", Serialize, Deserialize, dialect);
		}

		private static DbConnection CreateConnection(string connectionString)
		{
			var connection = new SqliteConnection(connectionString);
			connection.Open();
			using var command = connection.CreateCommand();
			command.CommandText = "pragma journal_mode=wal";
			command.ExecuteNonQuery();
			return connection;
		}

		private static void ConfigureWebServer(IWebHostBuilder builder)
		{
			static void ConfigureKestrel(WebHostBuilderContext c, KestrelServerOptions o)
			{
				var port = c.Configuration.GetValue("Host:Server:Port", 31337);
				o.Listen(IPAddress.Any, port);
			}

			builder.ConfigureKestrel(ConfigureKestrel);
			builder.UseStartup<Startup>();
		}

		private static void ReportException(Exception exception)
		{
			var message = string.Format(
				"Execution failed.\n{0}: {1}\n{2}",
				exception.GetType().Name,
				exception.Message,
				exception.StackTrace);
			MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			Log.Error(exception, "Host application failed");
		}
	}
}
