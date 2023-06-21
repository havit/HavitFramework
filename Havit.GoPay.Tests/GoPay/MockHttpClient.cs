using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Tests.GoPay;

public class MockHttpClient : HttpClient
{
	private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

	public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
	{
		fakeResponses.Add(uri, responseMessage);
	}

	public new virtual async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		return await Task.Run(() =>
		{
			if (fakeResponses.ContainsKey(request.RequestUri))
			{
				return fakeResponses[request.RequestUri];
			}
			else
			{
				return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
			}
		}, cancellationToken);
	}
}
