using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttpRemoting.Data;
using Newtonsoft.Json;
using RestEase;
using RestEase.Implementation;

namespace HttpRemoting.Client
{
	internal class HttpRemotingRequester: Requester
	{
		private readonly IHttpRemotingErrorAdapter _errorAdapter;

		public HttpRemotingRequester(
			HttpClient httpClient,
			IHttpRemotingErrorAdapter? errorAdapter = null,
			JsonSerializerSettings? serializationSettings = null):
			base(httpClient)
		{
			serializationSettings ??= new JsonSerializerSettings();
			_errorAdapter = errorAdapter ?? DefaultHttpRemotingErrorAdapter.Instance;

			RequestBodySerializer = new JsonRequestBodySerializer {
				JsonSerializerSettings = serializationSettings
			};
			RequestQueryParamSerializer = new JsonRequestQueryParamSerializer {
				JsonSerializerSettings = serializationSettings
			};
			ResponseDeserializer = new HttpRemotingResponseDeserializer(serializationSettings);
		}

		public override async Task RequestVoidAsync(IRequestInfo requestInfo)
		{
			var response = await RequestAny<JsonResponse>(requestInfo);
			Resolve(response.GetContent());
		}

		public override async Task<T> RequestAsync<T>(IRequestInfo requestInfo)
		{
			var response = await RequestAny<JsonResponse<T>>(requestInfo);
			return Resolve(response.GetContent());
		}

		public override async Task<string?> RequestRawAsync(IRequestInfo requestInfo)
		{
			var response = await RequestAny<JsonResponse<string>>(requestInfo);
			return Resolve(response.GetContent());
		}

		public override async Task<Response<T>> RequestWithResponseAsync<T>(
			IRequestInfo requestInfo)
		{
			var response = await RequestAny<JsonResponse<T>>(requestInfo);
			return new Response<T>(
				response.StringContent,
				response.ResponseMessage,
				() => Resolve(response.GetContent()));
		}
		
		private void Resolve(IJsonResponse response) => 
			FailIfNeeded(response);

		private T Resolve<T>(IJsonResponse<T> response)
		{
			FailIfNeeded(response.Error);
			return response.Result;
		}
		
		private void FailIfNeeded(IHttpRemotingError? error)
		{
			if (error == null) return;

			throw _errorAdapter.ToException(error) ?? new HttpRemotingException(error);
		}
		
		private void FailIfNeeded(IJsonResponse? response) => FailIfNeeded(response?.Error);

		private async Task<Response<T>> RequestAny<T>(IRequestInfo requestInfo)
		{
			var response = await base.RequestWithResponseAsync<T>(AnyStatusCode(requestInfo));
			var message = response.ResponseMessage;
			var content = response.StringContent;
			var isEmpty = string.IsNullOrWhiteSpace(content);

			if (isEmpty)
				throw NoDataReturned(message);

			return response;
		}

		private static IRequestInfo AnyStatusCode(IRequestInfo requestInfo) =>
			requestInfo.AllowAnyStatusCode
				? requestInfo
				: new AnyStatusCodeRequestInfo(requestInfo);

		private static Exception NoDataReturned(HttpResponseMessage response)
		{
			var isSuccess = response.IsSuccessStatusCode;
			var statusCode = isSuccess
				? HttpStatusCode.InternalServerError
				: response.StatusCode;
			var errorText = isSuccess
				? "Response contains no data. No error information provided."
				: $"Response contains no data. {response.ReasonPhrase ?? "No reason given"}.";

			return new HttpRemotingException(statusCode, null, errorText);
		}
	}
}
