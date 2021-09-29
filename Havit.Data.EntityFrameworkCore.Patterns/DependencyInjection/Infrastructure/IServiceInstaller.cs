using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure
{
	/// <summary>
	/// Třída pro registraci služeb do dependency injection controlleru.
	/// Pomáhá abstrahovat registraci do různých DI containerů.
	/// </summary>
	public interface IServiceInstaller<TLifetime>
	{
        #region AddService...
        /// <summary>
        /// Přidá transientní službu s implementací do DI containeru.
        /// </summary>
        void AddServiceTransient<TService, TImplementation>();

		/// <summary>
		/// Přidá transientní službu s implementací do DI containeru.
		/// </summary>
		void AddServiceTransient(Type serviceType, Type implementationType);

		/// <summary>
		/// Přidá službu s implementací do DI containeru jako singleton.
		/// </summary>
		void AddServiceSingleton<TService, TImplementation>();

		/// <summary>
		/// Přidá službu s implementací do DI containeru jako singleton.
		/// </summary>
		void AddServiceSingleton(Type serviceType, Type implementationType);

		/// <summary>
		/// Přidá službu s implementací do DI containeru s uvedeným lifetime.
		/// </summary>
		void AddService<TService, TImplementation>(TLifetime lifetime);

		/// <summary>
		/// Přidá službu s implementací do DI containeru s uvedeným lifetime.
		/// </summary>
		void AddService(Type serviceType, Type implementationType, TLifetime lifetime);

		/// <summary>
		/// Zaregistruje singleton s konkrétní instancí.
		/// </summary>
		void AddServiceSingletonInstance(Type serviceType, object implementation);

		/// <summary>
		/// Přidá služby (ev. jedinou službu) s implementací do DI containeru s uvedeným lifetime.
		/// Implementace je zaregistrována pod všechny uvedené interfaces.
		/// </summary>
		void AddServices(Type[] serviceTypes, Type implementationType, TLifetime lifetime);

		/// <summary>
		/// Přidá transientní služby (ev. jedinou službu) s implementací do DI containeru.
		/// Implementace je zaregistrována pod všechny uvedené interfaces.
		/// </summary>
		void AddServicesTransient(Type[] type, Type dataSourceType);
		#endregion

		#region TryAddService...

		/// <summary>
		/// Přidá transientní službu s implementací do DI containeru, pokud ještě není registrována.
		/// </summary>
		void TryAddServiceTransient<TService, TImplementation>();

		/// <summary>
		/// Přidá transientní službu s implementací do DI containeru, pokud ještě není registrována.
		/// </summary>
		void TryAddServiceTransient(Type serviceType, Type implementationType);

		/// <summary>
		/// Přidá službu s implementací do DI containeru jako singleton, pokud ještě není registrována.
		/// </summary>
		void TryAddServiceSingleton<TService, TImplementation>();

		/// <summary>
		/// Přidá službu s implementací do DI containeru jako singleton, pokud ještě není registrována.
		/// </summary>
		void TryAddServiceSingleton(Type serviceType, Type implementationType);

		/// <summary>
		/// Přidá službu s implementací do DI containeru s uvedeným lifetime, pokud ještě není registrována.
		/// </summary>
		void TryAddService<TService, TImplementation>(TLifetime lifetime);

		/// <summary>
		/// Přidá službu s implementací do DI containeru s uvedeným lifetime, pokud ještě není registrována.
		/// </summary>
		void TryAddService(Type serviceType, Type implementationType, TLifetime lifetime);
		#endregion

		/// <summary>
		/// Zaregistruje factory do DI containeru, pokud ještě není registrována.
		/// </summary>
		void TryAddFactory(Type factoryType);
	}
}
