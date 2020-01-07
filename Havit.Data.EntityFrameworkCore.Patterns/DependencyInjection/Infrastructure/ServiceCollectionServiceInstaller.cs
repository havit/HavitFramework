using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure
{
	/// <summary>
	/// Třída pro registraci služeb do dependency injection controlleru, kterým je ServiceCollection.
	/// </summary>
	internal class ServiceCollectionServiceInstaller : ServiceInstallerBase<ServiceLifetime>
	{
		private readonly IServiceCollection services;

		/// <inheritdoc/>
		protected override ServiceLifetime SingletonLifetime => ServiceLifetime.Singleton;

		/// <inheritdoc/>
		protected override ServiceLifetime TransientLifetime => ServiceLifetime.Transient;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ServiceCollectionServiceInstaller(IServiceCollection services)
		{
			this.services = services;
		}

		public override void AddFactory(Type factoryType)
		{
			// TODO JK: Implementovat
			//throw new NotImplementedException();
		}

		/// <inheritdoc/>
		public override void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
		}

		/// <inheritdoc/>
		public override void AddServiceSingletonInstance(Type serviceType, object implementation)
		{
			services.AddSingleton(new ServiceDescriptor(serviceType, implementation));
		}

		/// <inheritdoc/>
		protected override void AddMultipleServices(Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
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
				services.AddSingleton(serviceType, sp => sp.GetRequiredService(firstServiceTypeToRegister));
			}
		}

	}
}
