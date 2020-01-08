using Havit.Diagnostics.Contracts;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Extensions.DependencyInjection.Scanners;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Havit.Extensions.DependencyInjection
{
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
		public static IServiceCollection AddByServiceAttibute(this IServiceCollection serviceCollection, Assembly assembly)
		{
			return AddByServiceAttibute(serviceCollection, assembly, ServiceAttribute.DefaultProfile);
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
		public static IServiceCollection AddByServiceAttibute(this IServiceCollection serviceCollection, Assembly assembly, string[] profiles)
		{
			Contract.Requires<ArgumentNullException>(profiles != null, nameof(profiles));

			foreach (string profile in profiles)
			{
				AddByServiceAttibute(serviceCollection, assembly, profile);
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
		public static IServiceCollection AddByServiceAttibute(this IServiceCollection serviceCollection, Assembly assembly, string profile)
		{
			Contract.Requires<ArgumentNullException>(serviceCollection != null, nameof(serviceCollection));
			Contract.Requires<ArgumentNullException>(assembly != null, nameof(assembly));

			var itemsToRegister = AssemblyScanner.GetTypesWithServiceAttribute(assembly, profile);
			foreach (var itemToRegister in itemsToRegister)
			{
				var interfacesToRegister = TypeInterfacesExtractor.GetInterfacesToRegister(itemToRegister.Type);

				if (interfacesToRegister.Length == 0)
				{ 
					throw new InvalidOperationException(String.Format("Type {0} implements no interface to register.", itemToRegister.Type.FullName));
				}

				if (interfacesToRegister.Length == 1)
				{
					serviceCollection.Add(new ServiceDescriptor(interfacesToRegister.Single(), itemToRegister.Type, itemToRegister.Lifetime));
					continue;
				}

				if ((itemToRegister.Lifetime == ServiceLifetime.Transient))
				{
					foreach (var interfaceToRegister in interfacesToRegister)
					{
						serviceCollection.Add(new ServiceDescriptor(interfaceToRegister, itemToRegister.Type, itemToRegister.Lifetime /* =Transient */));
					}
				}
				else
				{
					// je Scoped nebo Singleton a zároveň máme více interfaces
					var firstInterfaceToRegister = interfacesToRegister.First();
					
					// registrace prvního interface
					serviceCollection.Add(new ServiceDescriptor(firstInterfaceToRegister, itemToRegister.Type, itemToRegister.Lifetime /* Scoped nebo Singleton */));

					// registrace druhého a dalšího interface
					foreach (var interfaceToRegister in interfacesToRegister.Skip(1) /* až od druhého */)
					{
						serviceCollection.AddSingleton(interfaceToRegister, sp => sp.GetRequiredService(firstInterfaceToRegister));
					}
				}
			}

			return serviceCollection;
		}
	
	}
}
