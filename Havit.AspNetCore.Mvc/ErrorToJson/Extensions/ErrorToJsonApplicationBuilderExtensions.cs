using Havit.AspNetCore.Mvc.ErrorToJson.Middlewares;

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
		ArgumentNullException.ThrowIfNull(app);

		return app.UseMiddleware<ErrorToJsonMiddleware>();
	}
}