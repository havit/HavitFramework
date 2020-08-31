using Havit.AspNetCore.Mvc.ExceptionMonitoring;
using Havit.AspNetCore.Mvc.ExceptionMonitoring.Formatters;
using Havit.AspNetCore.Mvc.ExceptionMonitoring.Processors;
using Havit.AspNetCore.Mvc.ExceptionMonitoring.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

// Správný namespace je Microsoft.Extensions.DependencyInjection!

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension metody pro registraci exception monitoringu.
	/// </summary>
    public static class ExceptionMonitoringServiceCollectionExtensions
    {
		/// <summary>
		/// Zaregistruje služby pro exception monitoring (vč. SmtpExceptionMonitoringProcessor a HttpRequestExceptionFormatter).
		/// </summary>
        public static void AddExceptionMonitoring(this IServiceCollection services, IConfiguration configurationRoot)
        {
            services.Configure<SmtpExceptionMonitoringOptions>(configurationRoot.GetSection("AppSettings:SmtpExceptionMonitoring"));
            services.TryAddSingleton<IExceptionMonitoringService, ExceptionMonitoringService>();
            services.TryAddSingleton<IExceptionMonitoringProcessor, SmtpExceptionMonitoringProcessor>();
            services.TryAddSingleton<IExceptionFormatter, HttpRequestExceptionFormatter>();
        }
    }
}
