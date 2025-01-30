using Hangfire.Common;
using Hangfire.Server;
using Havit.AspNetCore.ExceptionMonitoring.Services;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Notifies job failure using an <see cref="IExceptionMonitoringService"/>.
/// </summary>
public class ExceptionMonitoringAttribute : JobFilterAttribute, IServerFilter
{
	private readonly IExceptionMonitoringService exceptionMonitoringService;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ExceptionMonitoringAttribute(IExceptionMonitoringService exceptionMonitoringService)
	{
		this.exceptionMonitoringService = exceptionMonitoringService;
	}

	/// <inheritdoc />
	public void OnPerforming(PerformingContext filterContext)
	{
		// NOOP
	}

	/// <inheritdoc />
	public void OnPerformed(PerformedContext filterContext)
	{
		if ((filterContext.Exception != null) && !filterContext.ExceptionHandled)
		{
			exceptionMonitoringService.HandleException(filterContext.Exception.InnerException ?? filterContext.Exception);
		}
	}
}
