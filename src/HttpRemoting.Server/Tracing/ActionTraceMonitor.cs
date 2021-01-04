using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace HttpRemoting.Server.Tracing
{
	public class ActionTraceMonitor: IActionTraceMonitor
	{
		private static readonly TimeSpan DurationThreshold = TimeSpan.FromMilliseconds(500);

		private readonly ILogger? _logger;

		private readonly ConcurrentDictionary<string, long> _requestInfo =
			new ConcurrentDictionary<string, long>();

		private readonly ConcurrentDictionary<string, ActionStats> _actionStats =
			new ConcurrentDictionary<string, ActionStats>();

		private readonly long _baseTick;

		private long Ticks => Stopwatch.GetTimestamp() - _baseTick;

		private static double ElapsedBetween(long start, long stop) =>
			(double) (stop - start) / Stopwatch.Frequency;

		public ActionTraceMonitor(ILoggerFactory? loggers)
		{
			_logger = loggers?.CreateLogger(GetType());
			_baseTick = Ticks;
		}

		public void Enter(ActionExecutingContext context)
		{
			var requestId = GetRequestId(context);
			var requestInfo = GetRequestInfo(context);
			var stats = GetActionStats(context);
			_logger?.LogInformation(
				$"Enter(Id:{requestId}, Action:{stats.ActionName}, Request:{requestInfo})");
			stats.Enter();
			var requestStartTick = Ticks;
			_requestInfo.TryAdd(requestId, requestStartTick);
		}

		public void Leave(ActionExecutedContext context)
		{
			var requestStopTick = Ticks;
			var requestId = GetRequestId(context);
			var requestInfo = GetRequestInfo(context);
			_requestInfo.TryRemove(requestId, out var requestStartTick);
			var elapsedTime = TimeSpan.FromSeconds(ElapsedBetween(requestStartTick, requestStopTick));
			var stats = GetActionStats(context);
			_logger?.LogInformation(
				$"Leave(Id:{requestId}, Action:{stats.ActionName}, Request:{requestInfo})");
			var statsString = stats.Leave(context.Exception, elapsedTime, FormatStats);
			_logger?.Log(
				elapsedTime > DurationThreshold || context.Exception != null
					? LogLevel.Warning
					: LogLevel.Debug,
				$"Stats(Id:{requestId}, Action:{stats.ActionName}, Request:{requestInfo}, {statsString})");
		}

		private static string FormatStats(TimeSpan elapsed, ActionStats stats)
		{
			// ReSharper disable once UseStringInterpolation
			return string.Format(
				"Elapsed:{0:F}ms, Average:{1:F}ms, Total:{2}, Concurrent:{3}, MaxConcurrent:{4}, Failed:{5}/{6}%",
				elapsed.TotalMilliseconds,
				stats.TotalTime.TotalMilliseconds / stats.TotalCalls,
				stats.TotalCalls,
				stats.ConcurrentCalls,
				stats.MaxConcurrentCalls,
				stats.TotalFailed,
				stats.TotalFailed * 100 / stats.TotalCalls);
		}

		private static string GetActionId(ActionContext context) => context.ActionDescriptor.Id;
		private static string GetRequestId(ActionContext context) => context.HttpContext.TraceIdentifier;

		private ActionStats GetActionStats(ActionContext context) =>
			_actionStats.GetOrAdd(GetActionId(context), _ => new ActionStats(GetActionName(context)));

		private static string GetActionName(ActionContext context)
		{
			if (context.ActionDescriptor is not ControllerActionDescriptor controllerAction)
				return context.ActionDescriptor.DisplayName;

			var methodInfo = controllerAction.MethodInfo;
			var controller = ControllerName(methodInfo.DeclaringType);
			var method = MethodName(methodInfo);
			var parameters = string.Join(",", ParameterNames(methodInfo));
			return $"{controller}.{method}({parameters})";
		}

		private static string ControllerName(Type? controllerType) => 
			controllerType?.Name ?? "<unknown>";
		
		private static string MethodName(MethodInfo? methodInfo) => 
			methodInfo?.Name ?? "<unknown>";

		private static IEnumerable<string> ParameterNames(MethodInfo? methodInfo) =>
			methodInfo?.GetParameters().Select(p => p.Name) ?? Array.Empty<string>();

		private static string GetRequestInfo(ActionContext context)
		{
			var request = context.HttpContext.Request;
			return $"{request.Method} {request.Path}";
		}
	}
}
