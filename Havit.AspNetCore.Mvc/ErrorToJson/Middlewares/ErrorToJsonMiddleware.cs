using Havit.AspNetCore.Mvc.ErrorToJson.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Havit.AspNetCore.Mvc.ErrorToJson.Middlewares;

/// <summary>
/// Middleware for converting exceptions to object data.
/// </summary>
public class ErrorToJsonMiddleware
{
	private readonly RequestDelegate _next;
	private ILogger<ErrorToJsonMiddleware> _logger;
	private readonly IErrorToJsonService _errorToJsonService;
	private readonly IActionResultExecutor<ObjectResult> _executor;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ErrorToJsonMiddleware(RequestDelegate next, ILogger<ErrorToJsonMiddleware> logger, IErrorToJsonService errorToJsonService, IActionResultExecutor<ObjectResult> executor)
	{
		this._next = next;
		this._logger = logger;
		this._errorToJsonService = errorToJsonService;
		this._executor = executor;
	}

	/// <summary>
	/// Template method for the middleware pattern.
	/// </summary>
	public async Task Invoke(HttpContext context)
	{
		try
		{
			// call the next middleware
			await _next(context);
		}
		catch (Exception exception)
		{
			_logger.LogDebug(exception, "Handling exception.");

			if (context.Response.HasStarted)
			{
				_logger.LogWarning("The response has already started, the ErrorToJsonMiddleware will not be executed.");
				throw;
			}

			try
			{
				var resultData = _errorToJsonService.GetResultData(exception);
				if (resultData != null)
				{
					await WriteResultDataToResponse(context, resultData);

					if (resultData.ExceptionHandled)
					{
						_logger.LogTrace("Exception marked as handled.");
						return; // does not throw (nor re-throw) any exception
					}
					else
					{
						_logger.LogTrace("Exception not marked as handled.");
					}
				}
				else
				{
					_logger.LogTrace("Na data for json result.");
				}
			}
			catch (Exception handleExceptionException)
			{
				_logger.LogWarning(handleExceptionException, "An exception occured during exception handling.");
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

		return _executor.ExecuteAsync(actionContext, result);
	}
}