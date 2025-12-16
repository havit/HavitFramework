using Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Microsoft.Extensions.DependencyInjection;

// Správný namespace je Microsoft.AspNetCore.Builder!

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// <see cref="IApplicationBuilder"/> extension methods for the exception monitoring.
/// </summary>
public static class ExceptionMonitoringApplicationBuilderExtensions
{
	/// <summary>
	/// Adds IExceptionMonitoringService (DI) as registered handler of UnobservedTaskExceptionHandler.
	/// </summary>
	public static IApplicationBuilder UseUnobservedTaskExceptionHandler(this IApplicationBuilder app)
	{
		ArgumentNullException.ThrowIfNull(app);

		UnobservedTaskExceptionHandler.RegisterHandler(app.ApplicationServices.GetRequiredService<IExceptionMonitoringService>());

		return app;
	}

	/// <summary>
	/// Adds IExceptionMonitoringService (DI) as registered handler of AppDomainUnhandledExceptionHandler.
	/// </summary>
	public static IApplicationBuilder UseAppDomainUnhandledExceptionHandler(this IApplicationBuilder app)
	{
		ArgumentNullException.ThrowIfNull(app);

		AppDomainUnhandledExceptionHandler.RegisterHandler(app.ApplicationServices.GetRequiredService<IExceptionMonitoringService>());

		return app;
	}
}