using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;

namespace Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers;

internal class UnobservedTaskExceptionHandler
{
	private static UnobservedTaskExceptionHandler s_ExceptionHandler { get; set; }

	private readonly IExceptionMonitoringService _exceptionMonitoringService;

	public UnobservedTaskExceptionHandler(IExceptionMonitoringService exceptionMonitoringService)
	{
		this._exceptionMonitoringService = exceptionMonitoringService;
	}

	public static void RegisterHandler(IExceptionMonitoringService exceptionMonitoringService)
	{
		Contract.Requires<ArgumentNullException>(exceptionMonitoringService != null);

		if (s_ExceptionHandler != null)
		{
			throw new InvalidOperationException("Handler for unobserved task exception is already registered.");
		}

		var handler = new UnobservedTaskExceptionHandler(exceptionMonitoringService);
		s_ExceptionHandler = handler;

		TaskScheduler.UnobservedTaskException += handler.TaskScheduler_UnobservedTaskException;
	}

	private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
	{
		_exceptionMonitoringService.HandleException(e.Exception);
	}
}