using System;
using System.Threading.Tasks;

namespace TooMany.Host
{
	public static class Extensions
	{
		public static U PipeTo<T, U>(this T subject, Func<T, U> func) => func(subject);
		public static void PipeTo<T>(this T subject, Action<T> func) => func(subject);

		public static T TapWith<T, U>(this T subject, Func<T, U> func)
		{
			func(subject);
			return subject;
		}

		public static T TapWith<T>(this T subject, Action<T> func)
		{
			func(subject);
			return subject;
		}

		public static void Forget(this Task task) => task.ContinueWith(t => t.Exception);
	}
}
