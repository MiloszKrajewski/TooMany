using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using HttpRemoting.Client;
using K4os.RoutR;
using K4os.RoutR.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TooMany.Cli.Commands;
using TooMany.Cli.Handlers;
using SystemProcess = System.Diagnostics.Process;

namespace TooMany.Cli
{
	class Program
	{
		private static readonly Type ThisType = typeof(Program);
		private static readonly Assembly ThisAssembly = ThisType.Assembly;
		private static readonly string PackageName = ThisType.Namespace!;

		private static readonly string ProductName =
			ThisAssembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product!;

		private static readonly string AssemblyPath =
			Path.GetDirectoryName(SystemProcess.GetCurrentProcess().MainModule!.FileName)!;

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

		private static void Configure(ConfigurationBuilder builder) { }

		private static void Configure(ServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpClient();
			services.AddTransient(p => p.GetRequiredService<IHttpClientFactory>().CreateClient());
			services.AddTransient(
				p => {
					var port = configuration.GetValue("Host:Server:Port", 31337);
					var factory = p.GetRequiredService<IHttpClientFactory>();
					var client = factory.CreateClient();
					client.BaseAddress = new Uri($"http://127.0.0.1:{port}");
					return HttpRemotingFactory.Create<IHostInterface>(client);
				});
			services.AddTransient<ICommandHandler<GetLogsCommand>, GetLogsHandler>();
			services.AddTransient<ICommandHandler<ListTaskCommand>, TaskInfoHandler>();
			services.AddTransient<ICommandHandler<TaskInfoCommand>, TaskInfoHandler>();
			services.AddTransient<ICommandHandler<DefineTaskCommand>, DefineTaskHandler>();
			services.AddTransient<ICommandHandler<StartTaskCommand>, StartStopTaskHandler>();
			services.AddTransient<ICommandHandler<StopTaskCommand>, StartStopTaskHandler>();
			services.AddTransient<ICommandHandler<RemoveTaskCommand>, RemoveTaskHandler>();
			services.AddTransient<ICommandHandler<ApplyTagsCommand>, ApplyTagsHandler>();
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
				ApplyTagsCommand
			>(args);

			await parsed
				.WithNotParsed(e => log.LogError("Invalid arguments"))
				.WithParsedAsync(o => services.SendAny(o));

			return 0;
		}
	}
}
