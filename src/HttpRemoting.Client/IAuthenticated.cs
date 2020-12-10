using System;
using System.Net.Http.Headers;
using RestEase;

namespace HttpRemoting.Client
{
	public interface IAuthenticated
	{
		[Header("Authorization")]
		AuthenticationHeaderValue Authorization { get; set; }
	}
}
