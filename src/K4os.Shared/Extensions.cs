using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

		public static IEnumerable<T> NotNull<T>(this IEnumerable<T>? sequence) where T: class =>
			sequence ?? Array.Empty<T>();
		
		public static IReadOnlyCollection<T> NotNull<T>(this IReadOnlyCollection<T>? sequence) 
			where T: class =>
			sequence ?? Array.Empty<T>();

		public static T[] NotNull<T>(this T[]? sequence) where T: class =>
			sequence ?? Array.Empty<T>();

		public static string NotNull(this string? text) => text ?? string.Empty;

		public static string? NotBlank(this string? text, string? fallback = null) =>
			string.IsNullOrWhiteSpace(text) ? fallback : text;

		public static IEnumerable<T> NoNulls<T>(this IEnumerable<T?> sequence) where T: class =>
			sequence.Where(s => s != null).Select(s => s!);

		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? sequence) =>
			sequence ?? Array.Empty<T>();
		
		public static ICollection<T> EmptyIfNull<T>(this ICollection<T>? sequence) =>
			sequence ?? Array.Empty<T>();

		public static T[] EmptyIfNull<T>(this T[]? sequence) =>
			sequence ?? Array.Empty<T>();

		public static T? NullIfEmpty<T>(this T? sequence) where T: class, ICollection =>
			sequence is null || sequence.Count <= 0 ? default : sequence;
		
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

		public static void Forget(this Task task) => task.ContinueWith(t => t.Exception);

		public static string Join(this IEnumerable<string> sequence, string separator) =>
			string.Join(separator, sequence);

		public static string Quote(this string text, bool force = false) =>
			force || (text.Contains(' ') || text.Contains('\t') || string.IsNullOrWhiteSpace(text))
				? "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\""
				: text;

		public static string TruncateString(
			this string text, int length, bool tail = false)
		{
			if (text.Length <= length) return text;

			return tail
				? "..." + text.Substring(text.Length - length + 3, length - 3)
				: text.Substring(0, length - 3) + "...";
		}

		public static string TruncatePath(
			this string path, int length)
		{
			if (path.Length <= length) return path;

			[DllImport("shlwapi.dll")]
			static extern bool PathCompactPathEx(
				[Out] StringBuilder pszOut, string szPath, int cchMax, int dwFlags);

			var sb = new StringBuilder(length);
			PathCompactPathEx(sb, path, length, 0);
			return sb.ToString();
		}
	}
}
