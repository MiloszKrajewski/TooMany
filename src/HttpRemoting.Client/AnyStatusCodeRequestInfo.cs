using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using RestEase;
using RestEase.Implementation;

namespace HttpRemoting.Client
{
	internal class AnyStatusCodeRequestInfo: IRequestInfo
	{
		private readonly IRequestInfo _inner;
		public AnyStatusCodeRequestInfo(IRequestInfo inner) => _inner = inner;
		public bool AllowAnyStatusCode => true;
		public HttpMethod Method => _inner.Method;
		public string? BaseAddress => _inner.BaseAddress;
		public string? BasePath => _inner.BasePath;
		public string Path => _inner.Path;
		public CancellationToken CancellationToken => _inner.CancellationToken;
		public IEnumerable<QueryParameterInfo> QueryParams => _inner.QueryParams;
		public IEnumerable<RawQueryParameterInfo> RawQueryParameters => _inner.RawQueryParameters;
		public IEnumerable<PathParameterInfo> PathParams => _inner.PathParams;
		public IEnumerable<PathParameterInfo> PathProperties => _inner.PathProperties;
		public IEnumerable<QueryParameterInfo> QueryProperties => _inner.QueryProperties;

		public IEnumerable<HttpRequestMessagePropertyInfo> HttpRequestMessageProperties =>
			_inner.HttpRequestMessageProperties;

		public IEnumerable<KeyValuePair<string, string?>>? ClassHeaders => _inner.ClassHeaders;
		public IEnumerable<HeaderParameterInfo> PropertyHeaders => _inner.PropertyHeaders;
		public IEnumerable<KeyValuePair<string, string?>> MethodHeaders => _inner.MethodHeaders;
		public IEnumerable<HeaderParameterInfo> HeaderParams => _inner.HeaderParams;
		public BodyParameterInfo? BodyParameterInfo => _inner.BodyParameterInfo;
		public MethodInfo MethodInfo => _inner.MethodInfo;
	}
}
