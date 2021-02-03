using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Reflection;
using HttpRemoting.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HttpRemoting.Server
{
	public class HttpRemotingResponseBuilder: IHttpRemotingResponseBuilder
	{
		private static readonly JsonResponseInnerTypeCache InnerTypeCache =
			new JsonResponseInnerTypeCache();

		private readonly ConcurrentDictionary<string, HttpStatusCode> _actionInfo =
			new ConcurrentDictionary<string, HttpStatusCode>();

		private readonly ILogger _logger;

		public HttpRemotingResponseBuilder(ILoggerFactory? loggerFactory)
		{
			_logger = (loggerFactory ?? NullLoggerFactory.Instance).CreateLogger(GetType());
		}

		public void Execute(ActionExecutedContext context)
		{
			var services = context.HttpContext.RequestServices;

			if (context.Exception != null)
			{
				var exception = Unwrap(context.Exception)!;
				var error = Translate(exception, services)!;
				var failure = (int) error.StatusCode >= 500;
				var level = failure ? LogLevel.Error : LogLevel.Warning;
				_logger?.Log(level, exception, "Operation failed");
				ApplyError(context, error);
			}
			else if (context.Result is ObjectResult result)
			{
				ApplyResult(context, GetStatusCode(context), result.DeclaredType, result.Value);
			}
			else if (context.Result is EmptyResult)
			{
				ApplyEmptyResult(context, GetStatusCode(context));
			}
		}

		private static Exception Unwrap(Exception exception)
		{
			if (exception is not AggregateException aex) 
				return exception;

			var flat = aex.Flatten();
			var inner = flat.InnerExceptions;
			return inner.Count == 1 ? inner[0] : flat;
		}

		// ReSharper disable once SuspiciousTypeConversion.Global
		private static IHttpRemotingError? Translate(
			Exception? exception, IServiceProvider provider) =>
			exception is null ? null :
			exception is IHttpRemotingError error ? error :
			provider.GetService<IHttpRemotingErrorAdapter>()?.ToJsonError(exception) ??
			new JsonError(exception);

		private HttpStatusCode GetStatusCode(ActionContext context) =>
			_actionInfo.GetOrAdd(context.ActionDescriptor.Id, _ => FindStatusCode(context));

		private static HttpStatusCode FindStatusCode(ActionContext context) =>
			GetExplicitStatusCode(context.ActionDescriptor) ??
			GetStatusCodeByMethod(context.HttpContext.Request.Method) ??
			HttpStatusCode.OK;

		private static HttpStatusCode? GetExplicitStatusCode(ActionDescriptor descriptor) =>
			(descriptor as ControllerActionDescriptor)?
			.MethodInfo.GetCustomAttribute<StatusCodeAttribute>()?.StatusCode;

		private static HttpStatusCode? GetStatusCodeByMethod(string requestMethod)
		{
			switch (requestMethod)
			{
				case "GET": return HttpStatusCode.OK;
				case "POST": return HttpStatusCode.Created;
				case "DELETE": return HttpStatusCode.OK;
				case "PUT": return HttpStatusCode.OK;
				case "PATCH": return HttpStatusCode.OK;
				default: return null;
			}
		}

		private static void ApplyResult(
			ActionExecutedContext context, HttpStatusCode statusCode, Type type, object value)
		{
			switch (value)
			{
				case null:
					ApplyEmptyResult(context, statusCode);
					break;
				case IJsonResponse response:
					ApplyJsonResult(context, statusCode, type, response);
					break;
				default:
					ApplyValueResult(context, statusCode, type, value);
					break;
			}
		}

		private static void ApplyValueResult(
			ActionExecutedContext context, HttpStatusCode statusCode,
			Type type, object value)
		{
			context.Result = CreateResult(statusCode, JsonResponse.Create(type, value, null));
		}

		private static void ApplyJsonResult(
			ActionExecutedContext context, HttpStatusCode statusCode,
			Type type, IJsonResponse response)
		{
			var error = response.Error;
			var success = error is null;
			var value = success ? response.Result : null;
			var empty = value is null;
			
			Type InnerType() => InnerTypeCache.Get(type) ?? typeof(object);

			ObjectResult Create() =>
				!success ? CreateError(JsonResponse.FromError(error!)) :
				empty ? CreateResult(statusCode, JsonResponse.Empty) :
				CreateResult(statusCode, JsonResponse.Create(InnerType(), value, null));

			context.Result = Create();
		}

		private static void ApplyEmptyResult(
			ActionExecutedContext context, HttpStatusCode statusCode)
		{
			context.Result = CreateResult(statusCode, JsonResponse.Empty);
		}

		private static void ApplyError(
			ActionExecutedContext context, IHttpRemotingError error)
		{
			context.Result = CreateError(JsonResponse.FromError(error));
			context.ExceptionHandled = true;
		}

		private static ObjectResult CreateResult(
			HttpStatusCode statusCode, IJsonResponse result) =>
			new ObjectResult(result) {
				StatusCode = (int) (result.Error?.StatusCode ?? statusCode),
				DeclaredType = result.GetType(),
			};

		private static ObjectResult CreateError(IJsonResponse result) =>
			new ObjectResult(result) {
				StatusCode = (int) result.Error!.StatusCode,
				DeclaredType = result.GetType(),
			};
	}
}
