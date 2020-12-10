// using HttpRemoting.Data;
// using Newtonsoft.Json;
//
// namespace HttpRemoting.Server
// {
// 	public sealed class JsonResponseErrorOnly: IJsonResponse
// 	{
// 		[JsonProperty("result")]
// 		public object Result => null;
//
// 		[JsonProperty("error")]
// 		public JsonError Error { get; set; }
//
// 		[JsonConstructor]
// 		internal JsonResponseErrorOnly() { }
//
// 		internal JsonResponseErrorOnly(IHttpRemotingError error)
// 		{
// 			Error = WrapError(error);
// 		}
// 		
// 		private static JsonError WrapError(IHttpRemotingError error) =>
// 			error is null ? null :
// 			error is JsonError concrete ? concrete :
// 			new JsonError(error);
// 		
// 		IHttpRemotingError IJsonResponse.Error => Error;
// 	}
// }
