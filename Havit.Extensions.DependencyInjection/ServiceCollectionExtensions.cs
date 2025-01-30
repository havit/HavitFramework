﻿using System.Reflection;
using Havit.Diagnostics.Contracts;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Extensions.DependencyInjection.Scanners;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection;

/// <summary>
/// Service collection extensions to register services by ServiceAttribute.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds to the service collection classes from the given assembly marked by ServiceAttribute and having no defined profile (so it is ServiceAttribute.DefaultProfile).
	/// Classes are registered under interfaces "matching the class name", see TypeInterfacesExtractor.GetInterfacesToRegister. Multiple interfaces are supported.
	/// Lifetime is specified at the attribute level. 
	/// </summary>
	/// <param name="serviceCollection">Service collection where to add classes/interfaces registrations.</param>
	/// <param name="assembly">Assembly to scan for services.</param>
	/// <exception cref="InvalidOperationException">There is a class to register with no interface to register with.</exception>
	public static IServiceCollection AddByServiceAttribute(this IServiceCollection serviceCollection, Assembly assembly)
	{
		return AddByServiceAttribute(serviceCollection, assembly, ServiceAttribute.DefaultProfile);
	}

	/// <summary>
	/// Adds to the service collection classes from the given assembly marked by ServiceAttribute and having one of specified profiles.
	/// Classes are registered under interfaces "matching the class name", see TypeInterfacesExtractor.GetInterfacesToRegister. Multiple interfaces are supported.
	/// Lifetime is specified at the attribute level. 
	/// </summary>
	/// <param name="serviceCollection">Service collection where to add classes/interfaces registrations.</param>
	/// <param name="assembly">Assembly to scan for services.</param>
	/// <param name="profiles">Profiles to register.</param>
	/// <exception cref="InvalidOperationException">There is a class to register with no interface to register with.</exception>
	public static IServiceCollection AddByServiceAttribute(this IServiceCollection serviceCollection, Assembly assembly, string[] profiles)
	{
		Contract.Requires<ArgumentNullException>(profiles != null, nameof(profiles));

		foreach (string profile in profiles)
		{
			AddByServiceAttribute(serviceCollection, assembly, profile);
		}

		return serviceCollection;
	}

	/// <summary>
	/// Adds to the service collection classes from the given assembly marked by ServiceAttribute and having specified profile.
	/// Classes are registered under interfaces "matching the class name", see TypeInterfacesExtractor.GetInterfacesToRegister. Multiple interfaces are supported.
	/// Lifetime is specified at the attribute level. 
	/// </summary>
	/// <param name="serviceCollection">Service collection where to add classes/interfaces registrations.</param>
	/// <param name="assembly">Assembly to scan for services.</param>
	/// <param name="profile">Profile to register.</param>
	/// <exception cref="InvalidOperationException">There is a class to register with no interface to register with.</exception>
	public static IServiceCollection AddByServiceAttribute(this IServiceCollection serviceCollection, Assembly assembly, string profile)
	{
		Contract.Requires<ArgumentNullException>(serviceCollection != null, nameof(serviceCollection));
		Contract.Requires<ArgumentNullException>(assembly != null, nameof(assembly));

		TypeServiceAttributeInfo[] servicesToRegister = AssemblyScanner.GetTypesWithServiceAttribute(assembly, profile);
		foreach (TypeServiceAttributeInfo serviceToRegister in servicesToRegister)
		{
			Type[] serviceTypes = serviceToRegister.ServiceAttribute.GetServiceTypes() ?? TypeInterfacesExtractor.GetInterfacesToRegister(serviceToRegister.Type);

			if (serviceTypes.Length == 0)
			{
				throw new InvalidOperationException(String.Format("Type {0} implements no interface to register.", serviceToRegister.Type.FullName));
			}

			if (serviceTypes.Length == 1)
			{
				serviceCollection.Add(new ServiceDescriptor(serviceTypes.Single(), serviceToRegister.Type, serviceToRegister.ServiceAttribute.Lifetime));
				continue;
			}

			if ((serviceToRegister.ServiceAttribute.Lifetime == ServiceLifetime.Transient))
			{
				foreach (var interfaceToRegister in serviceTypes)
				{
					serviceCollection.Add(new ServiceDescriptor(interfaceToRegister, serviceToRegister.Type, serviceToRegister.ServiceAttribute.Lifetime /* =Transient */));
				}
			}
			else
			{
				// je Scoped nebo Singleton a zároveň máme více interfaces
				var firstInterfaceToRegister = serviceTypes.First();

				// registrace prvního interface
				serviceCollection.Add(new ServiceDescriptor(firstInterfaceToRegister, serviceToRegister.Type, serviceToRegister.ServiceAttribute.Lifetime /* Scoped nebo Singleton */));

				// registrace druhého a dalšího interface
				foreach (var interfaceToRegister in serviceTypes.Skip(1) /* až od druhého */)
				{
					serviceCollection.AddTransient(interfaceToRegister, sp => sp.GetRequiredService(firstInterfaceToRegister));
				}
			}
		}

		return serviceCollection;
	}

}
