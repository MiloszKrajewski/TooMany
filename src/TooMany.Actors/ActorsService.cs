using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto;
using TooMany.Actors.Catalog;

namespace TooMany.Actors
{
	public class ActorsService: IHostedService
	{
		private readonly IServiceProvider _services;
		private Func<Task> _stop;

		public ActorsService(IServiceProvider services)
		{
			_services = services;
			_stop = () => Task.CompletedTask;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var services = _services;
			var context = services.GetRequiredService<RootContext>();
			var catalogProps = services.GetRequiredProps<TaskCatalogActor>();

			var catalog = context.SpawnNamed(catalogProps, TaskCatalogActor.ActorName);

			_stop = () => context.StopAsync(catalog);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) => _stop();
	}
}
