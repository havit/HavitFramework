using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

/// <summary>
/// Extension metody pro IServiceCollection. Pro získání installeru Havit.Data.Entity.Patterns a souvisejících služeb.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Vrátí installer pro Havit.Data.Entity.Patterns a souvisejících služeb.
	/// </summary>
	/// <param name="services">ServiceCollection.</param>
	/// <param name="componentRegistrationAction">Konfigurace registrace komponent.</param>
	public static IEntityPatternsInstaller WithEntityPatternsInstaller(this IServiceCollection services, Action<ComponentRegistrationOptions> componentRegistrationAction = null)
	{
		ComponentRegistrationOptions componentRegistrationOptions = new ComponentRegistrationOptions();
		componentRegistrationAction?.Invoke(componentRegistrationOptions);
		return new EntityPatternsInstaller(services, componentRegistrationOptions);
	}

	internal static IServiceCollection AddServices(this IServiceCollection services, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
	{
		if (serviceTypes.Length == 0)
		{
			throw new ArgumentException("ServiceTypes required.", nameof(serviceTypes));
		}

		if (serviceTypes.Length == 1)
		{
			services.Add(new ServiceDescriptor(serviceTypes.Single(), implementationType, lifetime));
		}
		else
		{
			services.AddMultipleServices(serviceTypes, implementationType, lifetime);
		}

		return services;
	}

	internal static void AddMultipleServices(this IServiceCollection services, Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
	{
		if (lifetime == ServiceLifetime.Transient)
		{
			foreach (var serviceType in serviceTypes)
			{
				services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient));
			}
			return;
		}

		// je Scoped nebo Singleton a zároveň máme více interfaces
		var firstServiceTypeToRegister = serviceTypes.First();

		// registrace prvního interface
		services.Add(new ServiceDescriptor(firstServiceTypeToRegister, implementationType, lifetime /* Scoped nebo Singleton */));

		// registrace druhého a dalšího interface
		foreach (var serviceType in serviceTypes.Skip(1) /* až od druhého */)
		{
			services.AddTransient(serviceType, sp => sp.GetRequiredService(firstServiceTypeToRegister));
		}
	}
}
