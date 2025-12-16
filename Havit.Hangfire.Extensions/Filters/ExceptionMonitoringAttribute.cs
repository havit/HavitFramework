#if NET8_0_OR_GREATER
using Hangfire.Common;
using Hangfire.Server;
using Havit.AspNetCore.ExceptionMonitoring.Services;

namespace Havit.Hangfire.Extensions.Filters;

/// <summary>
/// Notifies job failure using an <see cref="IExceptionMonitoringService"/>.
/// </summary>
public class ExceptionMonitoringAttribute : JobFilterAttribute, IServerFilter
{
	private readonly IExceptionMonitoringService _exceptionMonitoringService;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ExceptionMonitoringAttribute(IExceptionMonitoringService exceptionMonitoringService)
	{
		this._exceptionMonitoringService = exceptionMonitoringService;
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
			_exceptionMonitoringService.HandleException(filterContext.Exception.InnerException ?? filterContext.Exception);
		}
	}
}
#endif