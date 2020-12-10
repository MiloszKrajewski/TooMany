using System;
using System.Net.Http;
using RestEase;

namespace HttpRemoting.Client
{
	public class HttpRemotingFactory
	{
		public static T Create<T>(HttpClient client) =>
			RestClient.For<T>(new HttpRemotingRequester(client));
	}
}
