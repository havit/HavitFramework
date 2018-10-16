using Castle.MicroKernel.Registration;
using Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Windsor.Installers
{
	/// <summary>
	/// Extension metody ke konfiguraci ComponentRegistrationOptions.
	/// </summary>
	public static class ComponentRegistrationOptionsExtensions
	{
		/// <summary>
		/// Řekne, že má být zaregistrována služba, která neprovádí žádné cachování.
		/// </summary>
		public static ComponentRegistrationOptions ConfigureNoCaching(this ComponentRegistrationOptions componentRegistrationOptions)
		{
			componentRegistrationOptions.CacheServiceInstaller = new NoCachingInstaller();
			return componentRegistrationOptions;
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba, která cachuje úplně všechno se sliding expirací.
		/// </summary>
		public static ComponentRegistrationOptions ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(this ComponentRegistrationOptions componentRegistrationOptions, TimeSpan slidingExpiration)
		{
			componentRegistrationOptions.CacheServiceInstaller = new CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(slidingExpiration);
			return componentRegistrationOptions;
		}

		/// <summary>
		/// Řekne, že má být zaregistrována služba vlastním installerem.
		/// </summary>
		public static ComponentRegistrationOptions ConfigureCustomCaching(this ComponentRegistrationOptions componentRegistrationOptions, IWindsorInstaller installer)
		{
			componentRegistrationOptions.CacheServiceInstaller = installer;
			return componentRegistrationOptions;
		}
	}
}
