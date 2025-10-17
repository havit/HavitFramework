#if NET8_0_OR_GREATER
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers;

/// <summary>
/// Exception handler (<see cref="IExceptionHandler"/> implementation) to report failed requests by <see cref="IExceptionMonitoringService"/>.
/// </summary>
public class ExceptionMonitoringExceptionHandler(
	IExceptionMonitoringService _exceptionMonitoringService,
	ILogger<ExceptionMonitoringExceptionHandler> _logger) : IExceptionHandler
{
	/// <inheritdoc />
	ValueTask<bool> IExceptionHandler.TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		try
		{
			exception.Data[nameof(HttpContext)] = httpContext;
			_exceptionMonitoringService.HandleException(exception);
		}
		catch (Exception handleExceptionException)
		{
			_logger.LogWarning(handleExceptionException, "An exception occured during exception handling.");
		}
		finally
		{
			exception.Data.Remove(nameof(HttpContext));
		}
		return ValueTask.FromResult(false);
	}
}
#endif