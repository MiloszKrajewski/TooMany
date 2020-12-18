using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
using TooMany.Actors.Worker.Processes;
using TooMany.Host.Frontend;
using TooMany.Host.Utilities;
using TooMany.WebServer;
using Log = Serilog.Log;
using SystemProcess = System.Diagnostics.Process;

namespace TooMany.Host
{
	internal static class Program
	{
		private static readonly Type ThisType = typeof(Program);
		private static readonly string AppName = "2many";
		private static readonly Guid AppGuid = Guid.Parse("e7e29a39-f7c8-4eec-b080-808495092a49");

		private static readonly string AssemblyPath =
			#if NET5_0
			AppContext.BaseDirectory;
			#else
			Path.GetDirectoryName(SystemProcess.GetCurrentProcess().MainModule!.FileName)!;
			#endif

		private static string _applicationDataPath = null!;

		private static string ApplicationDataPath =>
			_applicationDataPath ?? throw new InvalidOperationException(
				"ApplicationDataPath has not been configured");

		private static readonly CancellationTokenSource Cancel = new CancellationTokenSource();

		[STAThread]
		private static void Main()
		{
			try
			{
				using var systemLock = new SystemLock(
					ThisType.Namespace!, AppGuid, TimeSpan.FromSeconds(3));
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
				// because of the named mutex it cannot be async
				// mutex needs to be release from the same thread
				host.RunAsync(token).GetAwaiter().GetResult();
			}
			catch (SystemLockException e) when (e.Guid == AppGuid)
			{
				ReportSingleInstance();
			}
			catch (Exception e)
			{
				ReportException(e);
			}
		}

		private static void ConfigureHost(IConfigurationBuilder builder)
		{
			builder.AddCommandLine(Environment.GetCommandLineArgs());
			builder.AddEnvironmentVariables(@"2many_");
			builder.AddJsonFile($"{AppName}.json", true);
		}

		private static void ConfigureApp(
			HostBuilderContext context, IConfigurationBuilder builder)
		{
			// either: "--development true" or "set 2many_development true"
			var development = context.Configuration.GetValue("Development", false);
			var dataPath = development
				? context.HostingEnvironment.ContentRootPath
				: Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"TooMany");
			Directory.CreateDirectory(dataPath);

			_applicationDataPath = dataPath;
		}

		private static void ConfigureLogging(
			HostBuilderContext context, ILoggingBuilder builder)
		{
			const string outputTemplate =
				"{Timestamp:HH:mm:ss} [{Level:u4}] ({SourceContext:l}) {Message:lj}{NewLine}{Exception}";
			var outputFilename = Path.Combine(ApplicationDataPath, $"{AppName}.log");
			var logger = new LoggerConfiguration()
				.MinimumLevel.Warning()
				.MinimumLevel.Override(nameof(TooMany), LogEventLevel.Verbose)
				.Enrich.FromLogContext()
				.WriteTo.File(outputFilename, outputTemplate: outputTemplate)
				.CreateLogger();
			builder.AddSerilog(logger);
		}

		private static IServiceProviderFactory<IServiceCollection> ConfigureContainer(
			HostBuilderContext context) =>
			new DefaultServiceProviderFactory();

		private static void ConfigureServices(
			HostBuilderContext context, IServiceCollection services)
		{
			var connectionString = new SqliteConnectionStringBuilder {
				DataSource = Path.Combine(ApplicationDataPath, $"{AppName}.sqlite3"),
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
			services.AddTransient(_ => CreateConnection(connectionString));
			services.AddSingleton<IAnySqlDialect>(new SqLiteDialect());
			services.AddSingleton(CreatePersistence);
			services.AddSingleton(typeof(ITypedProps<>), typeof(TypedProps<>));
			services.AddSingleton<IReceiverMiddleware, DebugLoggingMiddleware>();
			services.AddTransient<IRealtimeService, RealtimeService>();
			
			services.AddSingleton<IProcessKiller, ProcessKiller>();
			services.AddSingleton<IProcessFactory, ProcessFactory>();

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

			object Deserialize(string s) =>
				JsonConvert.DeserializeObject(s, typeof(object), settings)!;

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

		private static void ReportSingleInstance()
		{
			MessageBox.Show(
				$"{ThisType.Namespace} is already running", "Error",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
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
