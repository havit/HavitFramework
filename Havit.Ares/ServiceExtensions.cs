using Havit.Ares.FinancniSprava;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Ares;

/// <summary>
/// Extensions k <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceExtensions
{
	/// <summary>
	/// Zaregistruje služby <see cref="IAresService"/>, <see cref="IPlatceDphService"/> a <see cref="IAresDphService"/> do DI containeru.
	/// </summary>
	public static IServiceCollection AddAresServices(this IServiceCollection services)
	{
		services.AddTransient<IAresService, AresService>();
		services.AddTransient<IPlatceDphService, PlatceDphService>();
		services.AddTransient<IAresDphService, AresDphService>();

		return services;
	}
}
