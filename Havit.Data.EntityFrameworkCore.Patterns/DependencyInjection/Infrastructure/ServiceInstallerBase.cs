using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure
{
	/// <summary>
	/// Třída pro registraci služeb do dependency injection controlleru.
	/// Abstrakrní implementace, která pomáhá implementovat v potomcích jen funkcionalitu specifickou jednotlivým containerům.
	/// </summary>
	public abstract class ServiceInstallerBase : IServiceInstaller
	{
		/// <inheritdoc />
		public abstract void TryAddFactory(Type factoryType);

        #region AddService...
        /// <inheritdoc />
        public void AddService<TService, TImplementation>(ServiceLifetime lifetime)
		{
			AddService(typeof(TService), typeof(TImplementation), lifetime);
		}

		/// <inheritdoc />
		public abstract void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime);

		/// <inheritdoc />
		public void AddServices(Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime)
		{
			if (serviceTypes.Length == 0)
			{
				throw new ArgumentException("ServiceTypes required.", nameof(serviceTypes));
			}

			if (serviceTypes.Length == 1)
			{
				AddService(serviceTypes.Single(), implementationType, lifetime);
			}
			else
			{
				AddMultipleServices(serviceTypes, implementationType, lifetime);
			}
		}

		/// <summary>
		/// Registrace více služeb k dané implementaci.
		/// Před voláním je ověřeno, že dochází k registraci minimálně dvou služeb.
		/// </summary>
		protected abstract void AddMultipleServices(Type[] serviceTypes, Type implementationType, ServiceLifetime lifetime);

		/// <inheritdoc />
		public void AddServiceSingleton<TService, TImplementation>()
		{
			AddService<TService, TImplementation>(ServiceLifetime.Singleton);
		}

		/// <inheritdoc />
		public void AddServiceSingleton(Type serviceType, Type implementationType)
		{
			AddService(serviceType, implementationType, ServiceLifetime.Singleton);
		}

		/// <inheritdoc />
		public void AddServiceScoped(Type serviceType, Type implementationType)
		{
			AddService(serviceType, implementationType, ServiceLifetime.Scoped);
		}

		/// <inheritdoc />
		public void AddServiceScoped<TService, TImplementation>()
		{
			AddService<TService, TImplementation>(ServiceLifetime.Scoped);
		}

		/// <inheritdoc />
		public void AddServiceTransient<TService, TImplementation>()
		{
			AddService<TService, TImplementation>(ServiceLifetime.Transient);			
		}

		/// <inheritdoc />
		public void AddServiceTransient(Type serviceType, Type implementationType)
		{
			AddService(serviceType, implementationType, ServiceLifetime.Transient);
		}

		/// <inheritdoc />
		public abstract void AddServiceSingletonInstance(Type serviceType, object implementation);

		/// <inheritdoc />
		public void AddServicesTransient(Type[] serviceTypes, Type implementationType)
		{
			AddServices(serviceTypes, implementationType, ServiceLifetime.Transient);
		}
        #endregion

        #region TryAddSevice
        /// <inheritdoc />
        public void TryAddServiceTransient<TService, TImplementation>()
        {
			TryAddService<TService, TImplementation>(ServiceLifetime.Transient);
        }

		/// <inheritdoc />
		public void TryAddServiceTransient(Type serviceType, Type implementationType)
        {
			TryAddService(serviceType, implementationType, ServiceLifetime.Transient);            
        }

		/// <inheritdoc />
		public void TryAddServiceScoped(Type serviceType, Type implementationType)
		{
			TryAddService(serviceType, implementationType, ServiceLifetime.Scoped);
		}

		/// <inheritdoc />
		public void TryAddServiceScoped<TService, TImplementation>()
		{
			TryAddService<TService, TImplementation>(ServiceLifetime.Scoped);
		}

		/// <inheritdoc />
		public void TryAddServiceSingleton<TService, TImplementation>()
        {
			TryAddService<TService, TImplementation>(ServiceLifetime.Singleton);
        }

		/// <inheritdoc />
		public void TryAddServiceSingleton(Type serviceType, Type implementationType)
        {
			TryAddService(serviceType, implementationType, ServiceLifetime.Singleton);
		}

		/// <inheritdoc />
		public void TryAddService<TService, TImplementation>(ServiceLifetime lifetime)
        {
			TryAddService(typeof(TService), typeof(TImplementation), lifetime);
		}

		/// <inheritdoc />
		public abstract void TryAddService(Type serviceType, Type implementationType, ServiceLifetime lifetime);
        #endregion
    }
}
