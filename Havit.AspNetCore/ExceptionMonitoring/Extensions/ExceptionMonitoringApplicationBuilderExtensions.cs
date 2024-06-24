using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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