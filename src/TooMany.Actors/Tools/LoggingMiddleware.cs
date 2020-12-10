using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Proto;
using NullLoggerFactory = Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory;

namespace TooMany.Actors.Tools
{
	public class LoggingMiddleware: IReceiverMiddleware
	{
		private static readonly JsonSerializerSettings ExplainJsonSettings =
			new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.None,
			};

		private readonly ILoggerFactory _loggerFactory;

		public LoggingMiddleware(ILoggerFactory? loggerFactory)
		{
			_loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
		}

		public async Task Handle(Receiver next, IReceiverContext context, MessageEnvelope envelope)
		{
			var logger = _loggerFactory.CreateLogger(LoggerName(context));

			try
			{
				Trace(logger, context, envelope.Message);
				await next(context, envelope);
			}
			catch (Exception exception)
			{
				Error(logger, context, envelope.Message, exception);
				throw;
			}
		}

		private static string LoggerName(IInfoContext context) =>
			(context.Actor?.GetType() ?? typeof(IActor)).FullName ?? "<unknown>";

		private static string ActorName(IInfoContext context) =>
			context.Self?.Id ?? "<unknown>";

		private void Trace(
			ILogger logger, IInfoContext context, object message)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			if (SuppressTrace(context, message)) return;

			logger.Log(
				LogLevel.Trace, 
				"[{0}] received {1}: {2}", 
				ActorName(context), TypeName(message), Explain(message));
		}

		protected virtual bool SuppressTrace(IInfoContext context, object message) => false;

		private static void Error(
			ILogger logger, IInfoContext context, object message, Exception exception)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;

			logger.Log(
				LogLevel.Error, exception, 
				"[{0}] failed while handling {1}: {2}", 
				ActorName(context), TypeName(message), Explain(message));
		}

		private static string TypeName(object? message) =>
			message?.GetType().FullName ?? "<unknown>";

		private static string Explain(object message) =>
			JsonConvert.SerializeObject(message, typeof(object), ExplainJsonSettings);
	}
}
