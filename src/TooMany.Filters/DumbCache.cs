using System;
using System.Collections.Generic;

namespace TooMany.Filters
{
	internal class DumbCache: ICacheAdapter
	{
		private readonly Dictionary<object, object?> _dictionary = new();

		public V? GetOrCreate<K, V>(K key, Func<K, V> factory)
		{
			if (key is null) return default;

			lock (_dictionary)
			{
				if (!_dictionary.TryGetValue(key, out var value))
					_dictionary[key] = value = factory(key);
				return (V?) value!;
			}
		}
	}
}
