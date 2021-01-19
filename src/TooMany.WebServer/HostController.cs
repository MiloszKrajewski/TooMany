using System;
using System.Linq;
using System.Threading.Tasks;
using HttpRemoting.Data;
using HttpRemoting.Server;
using HttpRemoting.Server.Tracing;
using K4os.Json.Messages.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proto;
using TooMany.Actors.Catalog;
using TooMany.Actors.Messages;
using TooMany.Messages;
using TooMany.WebServer.Messages;

namespace TooMany.WebServer
{
	[ApiController, Route("/api/v1"), HttpRemoting, ActionTrace, EnableCors]
	public class HostController: ActorController
	{
		protected ILogger Log { get; }

		public HostController(
			ActorSystem system,
			ILoggerFactory loggerFactory): base(system)
		{
			Log = loggerFactory.CreateLogger(GetType());
		}

		[HttpGet("now")]
		public DateTime Now() => DateTime.UtcNow;

		[HttpGet("empty")]
		public void Empty() { }

		[HttpGet("fail")]
		public DateTime Fail() => throw new BadRequest();

		[HttpGet("version")]
		public string Version() => "0.0.0";

		[HttpPost("task/{name}")]
		public async Task<IJsonResponse<TaskResponse>> CreateTask(
			[FromRoute] string name, [FromBody] TaskRequest request)
		{
			var command = request.ToCommand(name);
			var response = await RequestAsync(command);
			return response switch {
				TaskAction r => Result(r.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}
		
		[HttpPut("task/{name}/tags")]
		public async Task<IJsonResponse<TaskResponse>> UpdateTags(
			[FromRoute] string name, [FromBody] TagsRequest request)
		{
			var command = request.ToCommand(name);
			var response = await RequestAsync(command);
			return response switch {
				TaskSnapshot r => Result(r.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}

		[HttpGet("task")]
		public async Task<IJsonResponse<TaskResponse[]>> GetTasks(
			[FromQuery] string? filter = null)
		{
			var response = await RequestAsync(new GetTasks(filter));
			return response switch {
				ManyTasksSnapshot s => Result(s.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}

		[HttpGet("task/{name}")]
		public async Task<IJsonResponse<TaskResponse>> GetTask(string name)
		{
			var response = await RequestAsync(new GetTask { Name = name });
			return response switch {
				TaskSnapshot s => Result(s.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}

		[HttpDelete("task/{name}")]
		public async Task<IJsonResponse<TaskResponse>> RemoveTask(string name)
		{
			var response = await RequestAsync(new RemoveTask { Name = name });
			return response switch {
				TaskAction r => Result(r.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}
		
		[HttpPut("task/{name}/start")]
		public async Task<IJsonResponse<TaskResponse>> StartTask(
			string name, [FromQuery] bool? force)
		{
			var response = await RequestAsync(new StartTask { Name = name, Force = force });
			return response switch {
				TaskSnapshot s => Result(s.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}
		
		[HttpPut("task/{name}/stop")]
		public async Task<IJsonResponse<TaskResponse>> StopTask(string name)
		{
			var response = await RequestAsync(new StopTask { Name = name });
			return response switch {
				TaskSnapshot s => Result(s.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}
		
		[HttpGet("task/{name}/logs")]
		public async Task<IJsonResponse<LogEntryResponse[]>> GetTaskLog(string name)
		{
			var response = await RequestAsync(new GetLog { Name = name });
			return response switch {
				TaskLog l => Result(l.ToResponse()),
				TaskNotFound e => NotFound(e),
				IError e => BadRequest(e),
				var m => Unexpected(m),
			};
		}

		private Task<IResponse> RequestAsync(IRequest request) =>
			RequestAsync(TaskCatalogActor.ActorName, request, Timeout);
	}
}
