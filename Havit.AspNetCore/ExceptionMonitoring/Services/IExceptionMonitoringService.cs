using System;

namespace Havit.AspNetCore.ExceptionMonitoring.Services;

/// <summary>
/// Exception Monitoring.
/// </summary>
    public interface IExceptionMonitoringService
    {
	/// <summary>
	/// Zpracuje předanou výjimku.
	/// </summary>
        void HandleException(Exception exception);
    }