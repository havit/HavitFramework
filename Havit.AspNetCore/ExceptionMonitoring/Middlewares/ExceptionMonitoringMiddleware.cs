using Havit.AspNetCore.ExceptionMonitoring.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Havit.AspNetCore.ExceptionMonitoring.Middlewares;

/// <summary>
/// Middleware to report failed requests.
/// </summary>
#if NET8_0_OR_GREATER
[Obsolete("Starting in NET8, use exception handling with IExceptionHandler.")]
#endif
public class ExceptionMonitoringMiddleware
{
	private readonly RequestDelegate next;
	private ILogger<ExceptionMonitoringMiddleware> logger;
	private readonly IExceptionMonitoringService exceptionMonitoringService;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ExceptionMonitoringMiddleware(RequestDelegate next, ILogger<ExceptionMonitoringMiddleware> logger, IExceptionMonitoringService exceptionMonitoringService)
	{
		this.next = next;
		this.logger = logger;
		this.exceptionMonitoringService = exceptionMonitoringService;
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
			logger.LogDebug(exception, "Monitoring exception.");

			try
			{
				exceptionMonitoringService.HandleException(exception);
			}
			catch (Exception handleExceptionException)
			{
				logger.LogWarning(handleExceptionException, "An exception occured during exception handling.");
			}

			// re-throw the original exception
			throw;
		}
	}
}
