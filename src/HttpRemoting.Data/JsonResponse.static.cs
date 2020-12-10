using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace HttpRemoting.Data
{
	public partial class JsonResponse
	{
		public static readonly IJsonResponse Empty = FromResult<object>(null!);

		public static IJsonResponse Create(
			Type resultType, object? result, IHttpRemotingError? error) =>
			GetConstructor(resultType)(result, error);

		public static JsonResponse<T> FromResult<T>(T result) =>
			new JsonResponse<T>(result, null);

		public static JsonResponseErrorOnly FromError(IHttpRemotingError error) =>
			new JsonResponseErrorOnly(error);

		public static JsonResponse<T> FromError<T>(IHttpRemotingError error) =>
			new JsonResponse<T>(default!, error);

		private delegate IJsonResponse Constructor(object? result, IHttpRemotingError? error);

		private static readonly ConcurrentDictionary<Type, Constructor> Constructors =
			new ConcurrentDictionary<Type, Constructor>();

		private static IJsonResponse ConstructorProxy<T>(object result, IHttpRemotingError error) =>
			new JsonResponse<T>((T) result, error);

		private static Constructor GetConstructor(Type resultType) =>
			Constructors.GetOrAdd(resultType, NewConstructor);

		private static Constructor NewConstructor(Type resultType)
		{
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
			// ReSharper disable once PossibleNullReferenceException
			var actualMethod = typeof(JsonResponse)
				.GetMethod(nameof(ConstructorProxy), flags)
				.MakeGenericMethod(resultType);

			var resultArg = Expression.Parameter(typeof(object));
			var errorArg = Expression.Parameter(typeof(IHttpRemotingError));
			var body = Expression.Call(null, actualMethod, resultArg, errorArg);
			var lambda = Expression.Lambda<Constructor>(body, resultArg, errorArg);
			return lambda.Compile();
//
// #warning this can be much faster with complied lambda
// 			return (result, error) =>
// 				(IJsonResponse) actualMethod.Invoke(null, new[] { result, error });
		}
	}
}
