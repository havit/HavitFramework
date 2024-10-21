using Havit.AspNetCore.ExceptionMonitoring.ExceptionHandlers;
using Havit.AspNetCore.ExceptionMonitoring.Middlewares;
using Havit.AspNetCore.ExceptionMonitoring.Services;
using Havit.Diagnostics.Contracts;
using Microsoft.Extensions.DependencyInjection;

// Správný namespace je Microsoft.AspNetCore.Builder!

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// <see cref="IApplicationBuilder"/> extension methods for the <see cref="ExceptionMonitoringMiddleware"/>.
/// </summary>
public static class ExceptionMonitoringApplicationBuilderExtensions
{
	/// <summary>
	/// Adds a ExceptionMonitoringMiddleware to your web application pipeline to handle failed requests.
	/// </summary>
#if NET8_0_OR_GREATER
	[Obsolete("Starting in NET8, use exceptions handling with IExceptionHandler. Register services only by services.AddExceptionMonitoring() and let the app.UseExceptionHandler(...) to do its job. For more information about handling errors with IExceptionHandler, see https://medium.com/@AntonAntonov88/handling-errors-with-iexceptionhandler-in-asp-net-core-8-0-48c71654cc2e")]
#endif
	public static IApplicationBuilder UseExceptionMonitoring(this IApplicationBuilder app)
	{
		Contract.Requires<ArgumentNullException>(app != null, nameof(app));

		return app.UseMiddleware<ExceptionMonitoringMiddleware>();
	}

	/// <summary>
	/// Adds IExceptionMonitoringService (DI) as registered handler of UnobservedTaskExceptionHandler.
	/// </summary>
	public static IApplicationBuilder UseUnobservedTaskExceptionHandler(this IApplicationBuilder app)
	{
		Contract.Requires<ArgumentNullException>(app != null, nameof(app));

		UnobservedTaskExceptionHandler.RegisterHandler(app.ApplicationServices.GetRequiredService<IExceptionMonitoringService>());

		return app;
	}

	/// <summary>
	/// Adds IExceptionMonitoringService (DI) as registered handler of AppDomainUnhandledExceptionHandler.
	/// </summary>
	public static IApplicationBuilder UseAppDomainUnhandledExceptionHandler(this IApplicationBuilder app)
	{
		AppDomainUnhandledExceptionHandler.RegisterHandler(app.ApplicationServices.GetRequiredService<IExceptionMonitoringService>());

		return app;
	}
}