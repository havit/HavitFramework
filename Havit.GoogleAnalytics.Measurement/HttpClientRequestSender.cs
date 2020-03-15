using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics
{
    /// <summary>
    /// Implementation of <see cref="IHttpRequestSender"/> using HttpClient
    /// </summary>
    public class HttpClientRequestSender : IHttpRequestSender
    {
        /// <summary>
        ///  Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content of the request</param>
        /// <returns>Awaitable task</returns>
        public Task PostAsync(string requestUri, HttpContent content)
        {
            HttpClient httpClient = new HttpClient();
            return httpClient.PostAsync(requestUri, content).ContinueWith(x => httpClient.Dispose());
        }
    }
}
