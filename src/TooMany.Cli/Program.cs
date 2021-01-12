using System;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HttpRemoting.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using TooMany.Cli.Commands;
using TooMany.Cli.UserInterface;
using TooMany.Cli.Utilities;
using SystemProcess = System.Diagnostics.Process;

// ReSharper disable UnusedParameter.Local

namespace TooMany.Cli
{
	class Program
	{
		private static readonly string AppName = "2many";

		private static readonly string AssemblyPath =
			#if NET5_0
			AppContext.BaseDirectory;
			#else
			Path.GetDirectoryName(SystemProcess.GetCurrentProcess().MainModule!.FileName)!;
			#endif

		static async Task<int> Main(string[] args)
		{
			try
			{
				var serviceCollection = new ServiceCollection();

				var configurationBuilder = new ConfigurationBuilder();
				Configure(configurationBuilder);

				var configuration = configurationBuilder.Build();
				serviceCollection.AddSingleton<IConfiguration>(configuration);

				Configure(serviceCollection, configuration);

				return await Execute(serviceCollection, args);
			}
			catch (Exception e)
			{
				AnsiConsole.WriteException(e);
				return -1;
			}
		}

		private static void Configure(ConfigurationBuilder builder)
		{
			builder.AddEnvironmentVariables(@"2many_");
			builder.AddJsonFile(Path.Combine(AssemblyPath, $"{AppName}.json"), true);
		}

		private static void Configure(ServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpClient();
			services.AddTransient(p => p.GetRequiredService<IHttpClientFactory>().CreateClient());

			services.AddTransient(p => ConfigureHttpRemoting(p, configuration));
			services.AddTransient(p => ConfigureSignalR(p, configuration));
		}

		private static IHostInterface ConfigureHttpRemoting(
			IServiceProvider provider, IConfiguration configuration)
		{
			var addr = configuration.GetValue("Host:Server:Addr", "127.0.0.1");
			var port = configuration.GetValue("Host:Server:Port", 31337);
			var factory = provider.GetRequiredService<IHttpClientFactory>();
			var client = factory.CreateClient();
			client.BaseAddress = new Uri($"http://{addr}:{port}");
			return HttpRemotingFactory.Create<IHostInterface>(client);
		}

		private static HubConnection ConfigureSignalR(
			IServiceProvider provider, IConfiguration configuration)
		{
			var addr = configuration.GetValue("Host:Server:Addr", "127.0.0.1");
			var port = configuration.GetValue("Host:Server:Port", 31337);
			var uri = new Uri($"http://{addr}:{port}/monitor");
			return new HubConnectionBuilder()
				.WithUrl(uri)
				.AddNewtonsoftJsonProtocol()
				.Build();
		}

		private static async Task<int> Execute(
			ServiceCollection services, string[] args)
		{
			var app = new CommandApp(new SharedTypeRegistrar(services));

			app.Configure(
				config => {
					config.AddCommand<ListTasksCommand>("list");
					config.AddCommand<TaskDetailsCommand>("info");
					config.AddCommand<TaskSpecsCommand>("spec");
					config.AddCommand<GetLogsCommand>("logs");
					config.AddCommand<StartTaskCommand>("start");
					config.AddCommand<StopTaskCommand>("stop");
					config.AddCommand<RestartTaskCommand>("restart");
					config.AddCommand<MonitorCommand>("monitor");
					config.AddCommand<DefineTaskCommand>("define");
					config.AddCommand<ApplyTagsCommand>("tag");
					config.AddCommand<RemoveTaskCommand>("remove");
				});

			return await app.RunAsync(args);
		}
	}
}
