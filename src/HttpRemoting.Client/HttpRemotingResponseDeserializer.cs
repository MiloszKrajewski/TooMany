using System;
using System.Net;
using System.Net.Http;
using HttpRemoting.Data;
using Newtonsoft.Json;
using RestEase;

namespace HttpRemoting.Client
{
	internal class HttpRemotingResponseDeserializer: ResponseDeserializer
	{
		private readonly JsonSerializerSettings _settings;

		public HttpRemotingResponseDeserializer(JsonSerializerSettings settings) { _settings = settings; }

		public override T Deserialize<T>(
			string? content, HttpResponseMessage response, ResponseDeserializerInfo info)
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(content ?? string.Empty, _settings)!;
			}
			catch (Exception e)
			{
				throw DeserializationFailed(response, e);
			}
		}

		private static Exception DeserializationFailed(HttpResponseMessage response, Exception e)
		{
			var isSuccess = response.IsSuccessStatusCode;
			var statusCode = isSuccess
				? HttpStatusCode.InternalServerError
				: response.StatusCode;

			return new HttpRemotingException(
				statusCode, null, "Response cannot be deserialized", e);
		}
	}
}
