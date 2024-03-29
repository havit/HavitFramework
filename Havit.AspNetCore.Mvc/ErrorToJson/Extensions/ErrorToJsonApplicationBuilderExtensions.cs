﻿using Havit.AspNetCore.ExceptionMonitoring.Middlewares;
using Havit.AspNetCore.Mvc.ErrorToJson.Middlewares;
using Havit.Diagnostics.Contracts;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Správný namespace je Microsoft.AspNetCore.Builder!

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// <see cref="IApplicationBuilder"/> extension methods for the <see cref="ErrorToJsonMiddleware"/>.
/// </summary>
public static class ErrorToJsonApplicationBuilderExtensions
{
	/// <summary>
	/// Adds a ErrorToJsonMiddleware to your web application pipeline to handle exceptions.
	/// </summary>
	public static IApplicationBuilder UseErrorToJson(this IApplicationBuilder app)
	{
		Contract.Requires<ArgumentNullException>(app != null, nameof(app));

		return app.UseMiddleware<ErrorToJsonMiddleware>();
	}
}