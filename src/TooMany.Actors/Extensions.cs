#nullable enable

using System;
using System.Threading.Tasks;
using K4os.Json.Messages;
using K4os.Json.Messages.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Proto;
using TooMany.Actors.Tools;

namespace TooMany.Actors
{
	public static class Extensions
	{
		private static Task<bool> TaskTrue => Task.FromResult(true);

		public static Props? GetProps<TActor>(this IServiceProvider services)
			where TActor: IActor =>
			services.GetService<ITypedProps<TActor>>()?.Props;

		public static Props GetRequiredProps<TActor>(this IServiceProvider services)
			where TActor: IActor =>
			services.GetRequiredService<ITypedProps<TActor>>().Props;

		public static PID? GetNamed(this IInfoContext context, string name)
		{
			var system = context.System;
			var process = system.ProcessRegistry.GetLocal(name);
			return process == system.DeadLetter ? null : new PID(system.Address, name);
		}

		public static void SendLater(
			this IContext context,
			PID target, TimeSpan delay, Func<object> message)
		{
			context.ReenterAfter(Task.Delay(delay), () => context.Send(target, message()));
		}

		public static async Task Respond<T, R>(
			this IContext context, T message,
			Func<IContext, T, Task<R?>> handler)
			where R: class, IMessage
		{
			try
			{
				var response = await handler(context, message);
				if (response is null)
					context.Respond(new EmptyResponse());
				else
					context.Respond(response);
			}
			catch (ThrowableError e)
			{
				context.Respond(e.Error);
			}
			catch (Exception e)
			{
				context.Respond(new GenericError(e.Message));
			}
		}

		public static async Task Respond<T>(
			this IContext context, T message,
			Func<IContext, T, Task> handler)
			where T: IRequest
		{
			try
			{
				await handler(context, message);
				context.Respond(new EmptyResponse());
			}
			catch (ThrowableError e)
			{
				context.Respond(e.Error);
			}
			catch (Exception e)
			{
				context.Respond(new GenericError(e.Message));
			}
		}

		public static Task<bool> Forward(
			this IContext context, PID target, object message)
		{
			context.Request(target, message, context.Sender);
			return TaskTrue;
		}

		public static Task<bool> Return(
			this IContext context, object message, bool sendToParent = false)
		{
			if (sendToParent && context.Parent != null)
				context.Send(context.Parent, message);
			context.Respond(message!);
			return TaskTrue;
		}

		public static string? ToRelativeId(this IContext context, string absoluteId)
		{
			var prefix = context.Self!.Id;
			var length = prefix.Length;
			var isChild =
				absoluteId.StartsWith(prefix) &&
				absoluteId.Length > length &&
				absoluteId[length] == '/';
			return isChild ? absoluteId.Substring(length + 1) : null;
		}
	}
}
