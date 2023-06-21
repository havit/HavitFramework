using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoogleAnalytics;

    /// <summary>
    /// Abstracted http client
    /// </summary>
    public interface IHttpRequestSender
    {
        /// <summary>
        ///  Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">Request uri</param>
        /// <param name="content">Content of the request</param>
        /// <returns>Awaitable task</returns>
        Task PostAsync(string requestUri, HttpContent content);
    }
