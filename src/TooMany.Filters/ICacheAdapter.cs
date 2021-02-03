using System;

namespace TooMany.Filters
{
	public interface ICacheAdapter
	{
		public V? GetOrCreate<K, V>(K key, Func<K, V> factory);
	}
}
