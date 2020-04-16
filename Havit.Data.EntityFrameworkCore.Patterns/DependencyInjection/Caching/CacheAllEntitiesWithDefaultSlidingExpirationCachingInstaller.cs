using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching
{
	/// <summary>
	/// Installer, která konfiguraci služeb pro cachování. Cachovat se budu všechny objekty s definovanou sliding expirací.
	/// </summary>
	public class CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller<TLifetime> : DefaultCachingInstaller<TLifetime>
	{
		private readonly TimeSpan slidingExpiration;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(TimeSpan slidingExpiration)
		{
			this.slidingExpiration = slidingExpiration;
		}

		/// <inheritdoc />
		protected override void RegisterEntityCacheOptionsGenerator(IServiceInstaller<TLifetime> installer)
		{
			// no base call!

			AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions options = new AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions
			{
				SlidingExpiration = slidingExpiration,
				AbsoluteExpiration = null
			};

			installer.AddServiceTransient<IEntityCacheOptionsGenerator, AnnotationsWithDefaultsEntityCacheOptionsGenerator>();
			installer.AddServiceSingletonInstance(typeof(AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions), options);
			installer.AddServiceSingleton<IAnnotationsEntityCacheOptionsGeneratorStorage, AnnotationsEntityCacheOptionsGeneratorStorage>();
		}

		/// <inheritdoc />
		protected override void RegisterEntityCacheSupportDecision(IServiceInstaller<TLifetime> installer)
		{
			// no base call!
			installer.AddServiceSingleton<IEntityCacheSupportDecision, CacheAllEntitiesEntityCacheSupportDecision>();
		}
	}
}
