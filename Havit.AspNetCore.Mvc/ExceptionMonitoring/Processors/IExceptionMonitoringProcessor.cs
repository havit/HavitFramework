using System;

namespace Havit.AspNetCore.Mvc.ExceptionMonitoring.Processors
{
	/// <summary>
	/// Exception Monitoring Processor.
	/// </summary>
    public interface IExceptionMonitoringProcessor
    {
		/// <summary>
		/// Zpravuje výjimku zaslanou do exception monitoringu.
		/// </summary>
        void ProcessException(Exception exception);
    }
}
