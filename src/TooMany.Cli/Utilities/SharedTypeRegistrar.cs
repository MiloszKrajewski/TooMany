using System;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace TooMany.Cli.Utilities
{
	internal class SharedTypeRegistrar: ITypeRegistrar
	{
		private readonly IServiceCollection _collection;

		public SharedTypeRegistrar(IServiceCollection collection) => _collection = collection;

		public void Register(Type service, Type implementation)
		{
			_collection.AddTransient(service, implementation);
		}

		public void RegisterInstance(Type service, object implementation)
		{
			_collection.AddSingleton(service, implementation);
		}

		public ITypeResolver Build() =>
			new InternalTypeResolver(_collection.BuildServiceProvider());

		internal class InternalTypeResolver: ITypeResolver
		{
			private readonly IServiceProvider _provider;

			public InternalTypeResolver(IServiceProvider provider) => _provider = provider;

			public object? Resolve(Type? type) => type is null ? null : _provider.GetService(type);
		}
	}
}
