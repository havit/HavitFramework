using System;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;

/// <summary>
/// Installer, která konfiguraci služeb pro cachování. Cachovat se budu všechny objekty s definovanou sliding expirací.
/// </summary>
public class CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller : DefaultCachingInstaller
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
	protected override void RegisterEntityCacheOptionsGenerator(IServiceCollection services)
	{
		// no base call!

		AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions options = new AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions
		{
			SlidingExpiration = slidingExpiration,
			AbsoluteExpiration = null
		};

		services.AddTransient<IEntityCacheOptionsGenerator, AnnotationsWithDefaultsEntityCacheOptionsGenerator>();
		services.AddSingleton(typeof(AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions), options);
		services.AddSingleton<IAnnotationsEntityCacheOptionsGeneratorStorage, AnnotationsEntityCacheOptionsGeneratorStorage>();
	}

	/// <inheritdoc />
	protected override void RegisterEntityCacheSupportDecision(IServiceCollection services)
	{
		// no base call!
		services.AddSingleton<IEntityCacheSupportDecision, CacheAllEntitiesEntityCacheSupportDecision>();
	}
}
