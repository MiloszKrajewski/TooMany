using System;
using System.Threading.Tasks;
using RestEase;
using TooMany.Messages;

namespace TooMany.Cli
{
	public interface IHostInterface: IDisposable
	{
		[Get("api/v1/empty")]
		Task<DateTime> Empty();

		[Get("api/v1/now")]
		Task<DateTime> Now();

		[Get("api/v1/fail")]
		Task<DateTime> Fail();
		
		[Get("api/v1/task")]
		Task<TaskResponse[]> GetTasks();

		[Get("api/v1/task")]
		Task<TaskResponse[]> GetTasks([Query] string filter);

		[Post("api/v1/task/{name}")]
		Task<TaskResponse> CreateTask([Path] string name, [Body] TaskRequest request);

		[Delete("api/v1/task/{name}")]
		Task<TaskResponse> RemoveTask([Path] string name);

		[Put("api/v1/task/{name}/start")]
		Task<TaskResponse> StartTask([Path] string name, [Query("force")] bool? force = null);

		[Put("api/v1/task/{name}/stop")]
		Task<TaskResponse> StopTask([Path] string name);

		[Get("api/v1/task/{name}")]
		Task<TaskResponse> GetTask([Path] string name);

		[Get("api/v1/task/{name}/logs")]
		Task<LogEntryResponse[]> GetTaskLog([Path] string name);

		[Put("api/v1/task/{name}/tags")]
		Task<TaskResponse> SetTags([Path] string name, [Body] TagsRequest request);
	}
}
