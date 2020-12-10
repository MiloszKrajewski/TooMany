using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;

namespace TooMany.Host.Frontend
{
	internal class FrontendService: IHostedService
	{
		private readonly IServiceProvider _services;
		private Task? _loop;

		public FrontendService(IServiceProvider services)
		{
			_services = services;
		}

		public Task StartAsync(CancellationToken token)
		{
			_loop = Task.Factory.StartNew(RunFrontend, TaskCreationOptions.LongRunning);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken token) => _loop ?? Task.CompletedTask;

		private void RunFrontend()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FrontendContext(_services));
		}
	}
}
