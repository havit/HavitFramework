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
	public abstract class ServiceInstallerBase<TLifetime> : IServiceInstaller<TLifetime>
	{
		/// <summary>
		/// Lifetime pro singletony.
		/// </summary>
		protected abstract TLifetime SingletonLifetime { get; }

		/// <summary>
		/// Lifetime pro transientní služby.
		/// </summary>
		protected abstract TLifetime TransientLifetime { get; }

		/// <inheritdoc />
		public abstract void TryAddFactory(Type factoryType);

        #region AddService...
        /// <inheritdoc />
        public void AddService<TService, TImplementation>(TLifetime lifetime)
		{
			AddService(typeof(TService), typeof(TImplementation), lifetime);
		}

		/// <inheritdoc />
		public abstract void AddService(Type serviceType, Type implementationType, TLifetime lifetime);

		/// <inheritdoc />
		public void AddServices(Type[] serviceTypes, Type implementationType, TLifetime lifetime)
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
		protected abstract void AddMultipleServices(Type[] serviceTypes, Type implementationType, TLifetime lifetime);

		/// <inheritdoc />
		public void AddServiceSingleton<TService, TImplementation>()
		{
			AddService<TService, TImplementation>(this.SingletonLifetime);
		}

		/// <inheritdoc />
		public void AddServiceSingleton(Type serviceType, Type implementationType)
		{
			AddService(serviceType, implementationType, this.SingletonLifetime);
		}

		/// <inheritdoc />
		public void AddServiceTransient<TService, TImplementation>()
		{
			AddService<TService, TImplementation>(this.TransientLifetime);			
		}

		/// <inheritdoc />
		public void AddServiceTransient(Type serviceType, Type implementationType)
		{
			AddService(serviceType, implementationType, this.TransientLifetime);
		}

		/// <inheritdoc />
		public abstract void AddServiceSingletonInstance(Type serviceType, object implementation);

		/// <inheritdoc />
		public void AddServicesTransient(Type[] serviceTypes, Type implementationType)
		{
			AddServices(serviceTypes, implementationType, this.TransientLifetime);
		}
        #endregion

        #region TryAddSevice
        /// <inheritdoc />
        public void TryAddServiceTransient<TService, TImplementation>()
        {
			TryAddService<TService, TImplementation>(this.TransientLifetime);
        }

		/// <inheritdoc />
		public void TryAddServiceTransient(Type serviceType, Type implementationType)
        {
			TryAddService(serviceType, implementationType, this.TransientLifetime);            
        }

		/// <inheritdoc />
		public void TryAddServiceSingleton<TService, TImplementation>()
        {
			TryAddService<TService, TImplementation>(this.SingletonLifetime);
        }

		/// <inheritdoc />
		public void TryAddServiceSingleton(Type serviceType, Type implementationType)
        {
			TryAddService(serviceType, implementationType, this.SingletonLifetime);
		}

		/// <inheritdoc />
		public void TryAddService<TService, TImplementation>(TLifetime lifetime)
        {
			TryAddService(typeof(TService), typeof(TImplementation), lifetime);
		}

		/// <inheritdoc />
		public abstract void TryAddService(Type serviceType, Type implementationType, TLifetime lifetime);
		#endregion
	}
}
