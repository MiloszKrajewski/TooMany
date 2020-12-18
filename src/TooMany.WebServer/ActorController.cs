using System;
using System.Net;
using System.Threading.Tasks;
using HttpRemoting.Data;
using K4os.Json.Messages.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Proto;
using TooMany.Actors;

namespace TooMany.WebServer
{
	[EnableCors]
	public class ActorController: ControllerBase
	{
		protected RootContext Context { get; }
		protected TimeSpan Timeout { get; }

		public ActorController(ActorSystem system, TimeSpan? timeout = null)
		{
			Context = system.Root;
			Timeout = timeout ?? TimeSpan.FromSeconds(15);
		}

		protected async Task<IResponse> RequestAsync(
			string actorName, IRequest request, TimeSpan timeout)
		{
			var manager = Context.GetNamed(actorName);
			if (manager is null)
				throw new HttpRemotingException(
					HttpStatusCode.InternalServerError, null,
					$"Actor '{actorName}' could not be found");

			return await Context.RequestAsync<IResponse>(manager, request, timeout);
		}

		protected static JsonResponse<T> Result<T>(T response) =>
			JsonResponse.FromResult(response);

		protected static JsonResponseErrorOnly Error(HttpStatusCode statusCode, IError error) =>
			JsonResponse.FromError(
				new HttpRemotingException(
					statusCode, error.GetType().Name, error.ErrorMessage));

		protected static JsonResponseErrorOnly NotFound(IError error) =>
			Error(HttpStatusCode.NotFound, error);

		protected static JsonResponseErrorOnly BadRequest(IError error) =>
			Error(HttpStatusCode.BadRequest, error);

		protected static JsonResponseErrorOnly Unexpected(object message) =>
			JsonResponse.FromError(
				new HttpRemotingException(
					HttpStatusCode.InternalServerError,
					message.GetType().Name,
					"Unexpected message type"));
	}
}
