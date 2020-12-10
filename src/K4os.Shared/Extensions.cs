using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

// ReSharper disable CheckNamespace

namespace System
{
	public static class Extensions
	{
		public static IDictionary<K, V> CopyFrom<K, V>(
			this IDictionary<K, V> subject, IEnumerable<(K, V)> other)
		{
			foreach (var (key, value) in other) subject[key] = value;
			return subject;
		}

		public static IDictionary<K, V> CopyFrom<K, V>(
			this IDictionary<K, V> subject, IEnumerable<KeyValuePair<K, V>> other)
		{
			foreach (var kv in other) subject[kv.Key] = kv.Value;
			return subject;
		}

		public static void PipeTo<T>(this T subject, Action<T> action) => action(subject);

		public static R PipeTo<T, R>(this T subject, Func<T, R> action) => action(subject);

		public static T TapWith<T>(this T subject, Action<T> action)
		{
			action(subject);
			return subject;
		}

		public static T TapWith<T, R>(this T subject, Func<T, R> action)
		{
			_ = action(subject);
			return subject;
		}

		public static T TypedClone<T>(this T subject) where T: ICloneable => (T) subject.Clone();

		public static T[] TypedCloneAll<T>(this IEnumerable<T> subject) where T: ICloneable =>
			subject.Select(x => x.TypedClone()).ToArray();

		/// <summary>Rethrows the specified exception preserving stack-trace.
		/// Returning same exception is a syntactic trick to allow "fake" rethrowing it
		/// which stop code inspections complaining about not all paths returning a value.</summary>
		/// <typeparam name="T">Type of exception.</typeparam>
		/// <param name="exception">The exception.</param>
		/// <returns>Same exception (it does not return it though, as it throws it).</returns>
		public static T Rethrow<T>(this T exception)
			where T: Exception
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
			// it never actually gets returned, but allows to do some syntactic trick sometimes
			return exception;
		}

		public static IEnumerable<T> NoNulls<T>(this IEnumerable<T?> sequence) where T: class =>
			sequence.Where(s => s != null).Select(s => s!);

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T>? sequence) =>
			sequence ?? Array.Empty<T>();

		public static ICollection<T> NotNull<T>(this ICollection<T>? sequence) =>
			sequence ?? Array.Empty<T>();

		public static T[] NotNull<T>(this T[]? sequence) =>
			sequence ?? Array.Empty<T>();

		public static Dictionary<K, V> ToDictionary<K, V>(
			this IEnumerable<KeyValuePair<K, V>> dictionary,
			Func<K, K>? cloneKey = null,
			Func<V, V>? cloneValue = null)
		{
			cloneKey ??= (x => x);
			cloneValue ??= (x => x);
			return dictionary.ToDictionary(kv => cloneKey(kv.Key), kv => cloneValue(kv.Value));
		}

		public static IEnumerable<(int Index, T Value)> WithIndex<T>(
			this IEnumerable<T> sequence) => sequence.Select((v, i) => (i, v));
	}
}
