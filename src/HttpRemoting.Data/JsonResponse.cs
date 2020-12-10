using System;
using Newtonsoft.Json;

namespace HttpRemoting.Data
{
	public abstract partial class JsonResponse: IJsonResponse
	{
		[JsonProperty("error")]
		public JsonError? Error { get; set; }

		protected JsonResponse() { }

		protected JsonResponse(IHttpRemotingError? error) { Error = WrapError(error); }

		private static JsonError? WrapError(IHttpRemotingError? error) =>
			error is null ? null :
			error is JsonError concrete ? concrete :
			new JsonError(error);

		object? IJsonResponse.Result => null;

		IHttpRemotingError? IJsonResponse.Error => Error;
	}

	public sealed class JsonResponseErrorOnly: JsonResponse
	{
		[JsonConstructor, Obsolete("Serialization only, use JsonResponse.FromError(...) instead")]
		public JsonResponseErrorOnly() { }

		internal JsonResponseErrorOnly(IHttpRemotingError error): base(error) { }
	}

	public sealed class JsonResponse<T>: JsonResponse, IJsonResponse<T>
	{
		[JsonProperty("result")]
		public T Result { get; set; } = default!;

		[JsonConstructor, Obsolete("Serialization only, use JsonResponse.FromResult(...) instead")]
		public JsonResponse() { }

		internal JsonResponse(T value, IHttpRemotingError? error): base(error) { Result = value; }

		object? IJsonResponse.Result => Result;

		public static implicit operator JsonResponse<T>(JsonResponseErrorOnly other) =>
			new JsonResponse<T>(default!, other.Error);
	}
}
