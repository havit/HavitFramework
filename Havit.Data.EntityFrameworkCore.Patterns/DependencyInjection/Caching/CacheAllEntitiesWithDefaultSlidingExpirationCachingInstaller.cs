using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;

/// <summary>
/// Installer, která konfiguraci služeb pro cachování. Cachovat se budu všechny objekty s definovanou sliding expirací.
/// </summary>
public class CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller : DefaultCachingInstaller
{
	private readonly TimeSpan _slidingExpiration;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(TimeSpan slidingExpiration)
	{
		_slidingExpiration = slidingExpiration;
	}

	/// <inheritdoc />
	protected override void RegisterEntityCacheOptionsGenerator(IServiceCollection services)
	{
		// no base call!

		AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilderOptions options = new AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilderOptions
		{
			SlidingExpiration = _slidingExpiration,
			AbsoluteExpiration = null
		};

		services.TryAddSingleton<IEntityCacheOptionsGenerator, AnnotationsEntityCacheOptionsGenerator>();
		services.TryAddTransient<IAnnotationsEntityCacheOptionsGeneratorStorageBuilder, AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilder>();
		services.TryAddSingleton<AnnotationsWithDefaultsEntityCacheOptionsGeneratorStorageBuilderOptions>(options);
		services.TryAddSingletonFromScopedServiceProvider<IAnnotationsEntityCacheOptionsGeneratorStorage>(sp => sp.GetRequiredService<IAnnotationsEntityCacheOptionsGeneratorStorageBuilder>().Build());
	}

	/// <inheritdoc />
	protected override void RegisterEntityCacheSupportDecision(IServiceCollection services)
	{
		// no base call!
		services.TryAddSingleton<IEntityCacheSupportDecision, CacheAllEntitiesEntityCacheSupportDecision>();
	}
}
