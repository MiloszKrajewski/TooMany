using System;
using K4os.Json.Messages.Interfaces;

namespace K4os.Json.Messages
{
	public class ThrowableError: Exception
	{
		public IError Error { get; }

		public ThrowableError(IError error): base(error.ErrorMessage) { Error = error; }
	}

	public static class Extensions
	{
		public static ThrowableError Throwable(this IError error) =>
			error is null ? null : new ThrowableError(error);
	}
}
