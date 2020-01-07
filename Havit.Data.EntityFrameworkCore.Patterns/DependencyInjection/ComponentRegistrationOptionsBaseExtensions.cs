using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Extension metody ke konfiguraci ComponentRegistrationOptions.
	/// </summary>
	public static class ComponentRegistrationOptionsBaseExtensions
	{
		/// <summary>
		/// Řekne, že má být zaregistrována služba, která neprovádí žádné cachování.
		/// </summary>
		public static void ConfigureNoCaching<TLifetime>(this ComponentRegistrationOptionsBase<TLifetime> componentRegistrationOptions)
		{
			componentRegistrationOptions.CachingInstaller = new NoCachingInstaller<TLifetime>();
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba, která cachuje úplně všechno se sliding expirací.
		/// </summary>
		public static void ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching<TLifetime>(this ComponentRegistrationOptionsBase<TLifetime> componentRegistrationOptions, TimeSpan slidingExpiration)
		{
			componentRegistrationOptions.CachingInstaller = new CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller<TLifetime>(slidingExpiration);
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba vlastním installerem.
		/// </summary>
		public static void ConfigureCustomCaching<TLifetime>(this ComponentRegistrationOptionsBase<TLifetime> componentRegistrationOptions, ICachingInstaller<TLifetime> installer)
		{
			componentRegistrationOptions.CachingInstaller = installer;
		}
	}
}
