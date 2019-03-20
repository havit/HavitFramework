using Havit.AspNetCore.Mvc.ExceptionMonitoring.Middlewares;
using Havit.Diagnostics.Contracts;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Správný namespace je Microsoft.AspNetCore.Builder!

namespace Microsoft.AspNetCore.Builder
{
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
	}
}