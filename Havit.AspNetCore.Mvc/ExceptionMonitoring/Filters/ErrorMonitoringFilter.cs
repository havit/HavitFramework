using Microsoft.AspNetCore.Mvc.Filters;

namespace Havit.AspNetCore.Mvc.ExceptionMonitoring.Filters
{
	/// <summary>
	/// Filtr zajišťující oznámení výjimky do mechanismu "ExceptionMonitoringu".
	/// </summary>
    public class ErrorMonitoringFilter : ExceptionFilterAttribute
    {
        private readonly IExceptionMonitoringService exceptionMonitoringService;

		/// <summary>
		/// Konstruktor.
		/// </summary>
        public ErrorMonitoringFilter(IExceptionMonitoringService exceptionMonitoringService)
        {
            this.exceptionMonitoringService = exceptionMonitoringService;
        }

		/// <summary>
		/// Předá výjimku od "ExceptionMonitoringu".
		/// </summary>
        public override void OnException(ExceptionContext context)
        {
            exceptionMonitoringService.HandleException(context.Exception);
        }

    }
}
