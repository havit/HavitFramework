using System.Diagnostics.CodeAnalysis;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Havit.AspNetCore.Mvc.ExceptionMonitoring.Filters;

/// <summary>
/// Filtr zajišťující oznámení výjimky do mechanismu "ExceptionMonitoringu".
/// </summary>
[SuppressMessage("SonarLint", "S3376", Justification = "V ASP.NET Core MVC je toto zamýšleno jako globální filtr, pak se slovo Attribute na konci názvu nevyžaduje.")]
public class ErrorMonitoringFilter : ExceptionFilterAttribute
{
	private readonly IExceptionMonitoringService exceptionMonitoringService;
	private readonly ILogger<ErrorMonitoringFilter> logger;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ErrorMonitoringFilter(IExceptionMonitoringService exceptionMonitoringService, ILogger<ErrorMonitoringFilter> logger)
	{
		this.exceptionMonitoringService = exceptionMonitoringService;
		this.logger = logger;
	}

	/// <summary>
	/// Předá výjimku od "ExceptionMonitoringu".
	/// </summary>
	public override void OnException(ExceptionContext context)
	{
		logger.LogDebug(context.Exception, "Monitoring exception.");

		Exception exception = context.Exception;
		try
		{
			exception.Data[nameof(HttpContext)] = context.HttpContext;
			exceptionMonitoringService.HandleException(exception);
		}
		catch (Exception handleExceptionException)
		{
			logger.LogWarning(handleExceptionException, "An exception occured during exception handling.");
		}
		finally
		{
			exception.Data.Remove(nameof(HttpContext));
		}
	}

}
