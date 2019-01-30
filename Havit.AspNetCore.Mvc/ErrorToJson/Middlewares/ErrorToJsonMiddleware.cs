using Havit.AspNetCore.Mvc.ErrorToJson.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.AspNetCore.Mvc.ErrorToJson.Middlewares
{
	/// <summary>
	/// Middleware for converting exceptions to object data.
	/// </summary>
	public class ErrorToJsonMiddleware
	{
		private readonly RequestDelegate next;
		private ILogger<ErrorToJsonMiddleware> logger;
		private readonly IErrorToJsonService errorToJsonService;
		private readonly IActionResultExecutor<ObjectResult> executor;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ErrorToJsonMiddleware(RequestDelegate next, ILogger<ErrorToJsonMiddleware> logger, IErrorToJsonService errorToJsonService, IActionResultExecutor<ObjectResult> executor)
		{
			this.next = next;
			this.logger = logger;
			this.errorToJsonService = errorToJsonService;
			this.executor = executor;
		}

		/// <summary>
		/// Template method for the middleware pattern.
		/// </summary>
		public async Task Invoke(HttpContext context)
		{
			try
			{
				// call the next middleware
				await next(context);
			}
			catch (Exception exception)
			{
				logger.LogDebug(exception, "Handling exception.");

				if (context.Response.HasStarted)
				{
					logger.LogWarning("The response has already started, the ErrorToJsonMiddleware will not be executed.");
					throw;
				}

				try
				{
					var resultData = errorToJsonService.GetResultData(exception);
					if (resultData != null)
					{
						await WriteResultDataToResponse(context, resultData);

						if (resultData.ExceptionHandled)
						{
							logger.LogTrace("Exception marked as handled.");
							return; // does not throw (nor re-throw) any exception
						}
						else
						{
							logger.LogTrace("Exception not marked as handled.");
						}
					}
					else
					{
						logger.LogTrace("Na data for json result.");
					}
				}
				catch (Exception handleExceptionException)
				{
					logger.LogWarning(handleExceptionException, "An exception occured during exception handling.");
				}

				// re-throw the original exception
				throw;
			}
		}

		private Task WriteResultDataToResponse(HttpContext context, ResultData resultData)
		{
			HeaderDictionary headers = new HeaderDictionary();
			headers.Append(HeaderNames.CacheControl, "no-cache, no-store, must-revalidate");
			headers.Append(HeaderNames.Pragma, "no-cache");
			headers.Append(HeaderNames.Expires, "0");

			context.Response.Clear();
			context.Response.StatusCode = resultData.StatusCode;

			foreach (var header in headers)
			{
				context.Response.Headers.Add(header);
			}

			var routeData = context.GetRouteData() ?? new RouteData();
			var actionContext = new ActionContext(context, routeData, new ActionDescriptor());
			var result = new ObjectResult(resultData.Data)
			{
				StatusCode = resultData.StatusCode,
				DeclaredType = resultData.Data.GetType(),
			};

			result.ContentTypes.Add("application/problem+json");
			result.ContentTypes.Add("application/problem+xml");
			result.ContentTypes.Add("application/json");
			result.ContentTypes.Add("application/xml");

			return executor.ExecuteAsync(actionContext, result);
		}
	}
}