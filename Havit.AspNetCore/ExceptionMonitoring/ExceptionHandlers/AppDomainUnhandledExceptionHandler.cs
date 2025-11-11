using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;

namespace Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers;

internal class AppDomainUnhandledExceptionHandler
{
	private readonly IExceptionMonitoringService _exceptionMonitoringService;

	private static AppDomainUnhandledExceptionHandler s_ExceptionHandler { get; set; }

	public AppDomainUnhandledExceptionHandler(IExceptionMonitoringService exceptionMonitoringService)
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

		var handler = new AppDomainUnhandledExceptionHandler(exceptionMonitoringService);
		s_ExceptionHandler = handler;

		AppDomain.CurrentDomain.UnhandledException += handler.CurrentDomain_UnhandledException;
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		if (e.ExceptionObject is Exception exception)
		{
			_exceptionMonitoringService.HandleException(exception);
		}
	}
}