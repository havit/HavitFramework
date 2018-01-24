using System;

namespace Havit.AspNetCore.Mvc.ExceptionMonitoring
{
	/// <summary>
	/// Exception Monitoring.
	/// </summary>
    public interface IExceptionMonitoringService
    {
		/// <summary>
		/// Zpracuje pøedanou výjimku.
		/// </summary>
        void HandleException(Exception exception);
    }
}