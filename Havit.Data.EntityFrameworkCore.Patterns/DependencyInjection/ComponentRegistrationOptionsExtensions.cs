using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection
{
	/// <summary>
	/// Extension metody ke konfiguraci ComponentRegistrationOptions.
	/// </summary>
	public static class ComponentRegistrationOptionsExtensions
	{
		/// <summary>
		/// Řekne, že má být zaregistrována služba, která neprovádí žádné cachování.
		/// </summary>
		public static void ConfigureNoCaching(this ComponentRegistrationOptions componentRegistrationOptions)
		{
			componentRegistrationOptions.CachingInstaller = new NoCachingInstaller();
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba, která cachuje úplně všechno se sliding expirací.
		/// </summary>
		public static void ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(this ComponentRegistrationOptions componentRegistrationOptions, TimeSpan slidingExpiration)
		{
			componentRegistrationOptions.CachingInstaller = new CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(slidingExpiration);
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba vlastním installerem.
		/// </summary>
		public static void ConfigureCustomCaching(this ComponentRegistrationOptions componentRegistrationOptions, ICachingInstaller installer)
		{
			componentRegistrationOptions.CachingInstaller = installer;
		}
	}
}
