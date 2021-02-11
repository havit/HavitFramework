using System;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;

namespace Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers
{
    internal class AppDomainUnhandledExceptionHandler
    {
        private readonly IExceptionMonitoringService exceptionMonitoringService;

        private static AppDomainUnhandledExceptionHandler ExceptionHandler { get; set; }

        public AppDomainUnhandledExceptionHandler(IExceptionMonitoringService exceptionMonitoringService)
        {
            this.exceptionMonitoringService = exceptionMonitoringService;
        }

        public static void RegisterHandler(IExceptionMonitoringService exceptionMonitoringService)
        {
            Contract.Requires<ArgumentNullException>(exceptionMonitoringService != null);

            if (ExceptionHandler != null)
            {
                throw new InvalidOperationException("Handler for unobserved task exception is already registered.");
            }

            var handler = new AppDomainUnhandledExceptionHandler(exceptionMonitoringService);
            ExceptionHandler = handler;

            AppDomain.CurrentDomain.UnhandledException += handler.CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                exceptionMonitoringService.HandleException(exception);
            }
        }
    }
}