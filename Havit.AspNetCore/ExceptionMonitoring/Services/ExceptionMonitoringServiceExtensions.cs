using Microsoft.AspNetCore.Http;

namespace Havit.AspNetCore.ExceptionMonitoring.Services;

/// <summary>
/// Extension metody k <see cref="IExceptionMonitoringService"/>.
/// </summary>
public static class ExceptionMonitoringServiceExtensions
{
	/// <summary>
	/// Zpracuje předanou výjimku.
	/// Přidá k výjimce jako kontext předaný httpContext.
	/// </summary>
	public static void HandleException(this IExceptionMonitoringService exceptionMonitoringService, Exception exception, HttpContext httpContext)
	{
		exception.Data[nameof(HttpContext)] = httpContext;
		try
		{
			exceptionMonitoringService.HandleException(exception);
		}
		finally
		{
			exception.Data.Remove(nameof(HttpContext));
		}
	}
}
