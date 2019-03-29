using System;
using System.Collections.Generic;
using System.Linq;
using Havit.AspNetCore.Mvc.ExceptionMonitoring.Filters;
using Havit.AspNetCore.Mvc.ExceptionMonitoring.Processors;
using Microsoft.Extensions.Logging;

namespace Havit.AspNetCore.Mvc.ExceptionMonitoring.Services
{
	/// <summary>
	/// Exception monitoring.
	/// Zpracuje výjimku předáním exception monitoring processorům.
	/// </summary>
    public class ExceptionMonitoringService : IExceptionMonitoringService
    {
        private readonly IExceptionMonitoringProcessor[] exceptionMonitoringProcessors;
        private readonly ILogger<ErrorMonitoringFilter> logger;

		/// <summary>
		/// Konstruktor.
		/// </summary>
        public ExceptionMonitoringService(IEnumerable<IExceptionMonitoringProcessor> exceptionMonitoringProcessors, ILogger<ErrorMonitoringFilter> logger)
        {
            this.exceptionMonitoringProcessors = exceptionMonitoringProcessors.ToArray();
            this.logger = logger;
        }

		/// <summary>
		/// Zpracuje výjimku předáním exception monitoring processorům.
		/// </summary>
        public void HandleException(Exception exception)
        {
            using (logger.BeginScope("Exception Monitoring"))
            {
                logger.LogDebug(0, "Processing exception of type {TYPE} with message '{MESSAGE}'.", exception.GetType().FullName, exception.Message);
                bool shouldMonitorException = ShouldHandleException(exception);
                logger.LogDebug("Exception ShouldBeMonitored = {SHOULDBEMONITORED}.", shouldMonitorException);

                if (ShouldHandleException(exception))
                {
                    if (exceptionMonitoringProcessors.Length == 0)
                    {
                        logger.LogWarning("No exception monitor registered.");
                    }

                    foreach (IExceptionMonitoringProcessor exceptionMonitor in exceptionMonitoringProcessors)
                    {
                        try
                        {
                            logger.LogDebug("Processing exception monitor {TYPE}.", exception.GetType().FullName);
                            exceptionMonitor.ProcessException(exception);
                        }
                        catch (Exception raisedException)
                        {
                            logger.LogError(0, raisedException, "Exception monitor {TYPE} failed with message {MESSAGE}", raisedException.GetType().FullName, raisedException.Message);
                        }
                    }
                }
            }
        }

		/// <summary>
		/// Vrací true, pokud se má výjimka zpracovávat (předávat procesorům).
		/// Vždy vrací true, ale umožňuje potomkům chování předefinovat.
		/// </summary>
        protected virtual bool ShouldHandleException(Exception exception)
        {
            return true;
        }

    }
}
