using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommandLine;
using HttpRemoting.Client;
using K4os.RoutR;
using K4os.RoutR.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.Handlers;
using SystemProcess = System.Diagnostics.Process;
// ReSharper disable UnusedParameter.Local

namespace TooMany.Cli
{
	class Program
	{
		private static readonly string AssemblyPath =
			#if NET5_0
			AppContext.BaseDirectory;
			#else
			Path.GetDirectoryName(SystemProcess.GetCurrentProcess().MainModule!.FileName)!;
			#endif

		static async Task<int> Main(string[] args)
		{
			var console = new ColorConsoleProvider("TooMany");

			try
			{
				var loggerFactory = new LoggerFactory();
				loggerFactory.AddProvider(console);

				var serviceCollection = new ServiceCollection();
				serviceCollection.AddSingleton<ILoggerFactory>(loggerFactory);

				var configurationBuilder = new ConfigurationBuilder();
				Configure(configurationBuilder);

				var configuration = configurationBuilder.Build();
				serviceCollection.AddSingleton<IConfiguration>(configuration);

				Configure(serviceCollection, configuration);
				var serviceProvider = serviceCollection.BuildServiceProvider();

				return await Execute(loggerFactory, serviceProvider, args);
			}
			catch (Exception e)
			{
				console.Default.LogError(e, "Operation failed");
				return -1;
			}
		}

		private static void Configure(ConfigurationBuilder builder)
		{
			#warning configure configuration (go to server settings json)
		}

		private static void Configure(ServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpClient();
			services.AddTransient(p => p.GetRequiredService<IHttpClientFactory>().CreateClient());
			
			services.AddTransient(p => ConfigureHttpRemoting(p, configuration));
			services.AddTransient(p => ConfigureSignalR(p, configuration));
			
			services.AddTransient<ICommandHandler<GetLogsCommand>, GetLogsHandler>();
			services.AddTransient<ICommandHandler<ListTaskCommand>, TaskInfoHandler>();
			services.AddTransient<ICommandHandler<TaskInfoCommand>, TaskInfoHandler>();
			services.AddTransient<ICommandHandler<DefineTaskCommand>, DefineTaskHandler>();
			services.AddTransient<ICommandHandler<StartTaskCommand>, StartStopTaskHandler>();
			services.AddTransient<ICommandHandler<StopTaskCommand>, StartStopTaskHandler>();
			services.AddTransient<ICommandHandler<RemoveTaskCommand>, RemoveTaskHandler>();
			services.AddTransient<ICommandHandler<ApplyTagsCommand>, ApplyTagsHandler>();
			services.AddTransient<ICommandHandler<MonitorCommand>, MonitorHandler>();
		}

		private static IHostInterface ConfigureHttpRemoting(
			IServiceProvider provider, IConfiguration configuration)
		{
			var port = configuration.GetValue("Host:Server:Port", 31337);
			var factory = provider.GetRequiredService<IHttpClientFactory>();
			var client = factory.CreateClient();
			client.BaseAddress = new Uri($"http://localhost:{port}");
			return HttpRemotingFactory.Create<IHostInterface>(client);
		}

		private static HubConnection ConfigureSignalR(
			IServiceProvider provider, IConfiguration configuration)
		{
			var port = configuration.GetValue("Host:Server:Port", 31337);
			var uri = new Uri($"http://localhost:{port}/monitor");
			return new HubConnectionBuilder()
				.WithUrl(uri)
				.AddNewtonsoftJsonProtocol()
				.Build();
		}

		private static async Task<int> Execute(
			LoggerFactory loggerFactory, ServiceProvider services, string[] args)
		{
			var log = loggerFactory.CreateLogger(typeof(Program));

			var parser = Parser.Default;
			var parsed = parser.ParseArguments<
				GetLogsCommand,
				ListTaskCommand,
				TaskInfoCommand,
				DefineTaskCommand,
				StartTaskCommand,
				StopTaskCommand,
				RemoveTaskCommand,
				ApplyTagsCommand,
				MonitorCommand
			>(args);

			await parsed
				.WithNotParsed(e => log.LogError("Invalid arguments"))
				.WithParsedAsync(o => services.SendAny(o));

			return 0;
		}
	}
}
