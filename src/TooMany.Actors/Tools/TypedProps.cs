using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Proto;

namespace TooMany.Actors.Tools
{
	public class TypedProps<T>: ITypedProps<T> where T: IActor
	{
		private const LogLevel TraceLevel = LogLevel.Warning;

		// ReSharper disable once StaticMemberInGenericType
		private static readonly JsonSerializerSettings ExplainJsonSettings =
			new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto,
			};

		public TypedProps(
			IServiceProvider provider,
			IEnumerable<IReceiverMiddleware> middleware,
			ILoggerFactory loggerFactory)
		{
			IActor NewInstance() =>
				provider.CreateScope().ServiceProvider.GetRequiredService<T>();

			Props = Props
				.FromProducer(NewInstance)
				.WithReceiverMiddleware(middleware.Select(MakeReceiverMiddleware).ToArray());
		}

		private static Func<Receiver, Receiver> MakeReceiverMiddleware(
			IReceiverMiddleware middleware) =>
			next => (context, envelope) => middleware.Handle(next, context, envelope);

		public Props Props { get; }
	}
}
