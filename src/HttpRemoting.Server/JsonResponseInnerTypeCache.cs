using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HttpRemoting.Data;

namespace HttpRemoting.Server
{
	internal class JsonResponseInnerTypeCache
	{
		private readonly ConcurrentDictionary<Type, Type> _dictionary =
			new ConcurrentDictionary<Type, Type>();

		public Type Get(Type type) => _dictionary.GetOrAdd(type, Resolve);

		private static Type Resolve(Type type)
		{
			var isJsonResponse = typeof(IJsonResponse).IsAssignableFrom(type);
			if (!isJsonResponse) return null;

			var allImplemented = ResolveMany(type).ToArray();
			return allImplemented.Length != 1 ? typeof(object) : allImplemented[0];
		}

		private static IEnumerable<Type> ResolveMany(Type type)
		{
			if (!type.IsInterface || !type.IsGenericType)
				return type.GetInterfaces().SelectMany(ResolveMany);
			if (type.GetGenericTypeDefinition() != typeof(IJsonResponse<>))
				return Array.Empty<Type>();

			return type.GetGenericArguments();
		}
	}
}
