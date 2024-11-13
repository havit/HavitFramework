using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;

/// <summary>
/// Installer, která výchozí konfiguraci služeb pro cachování (EntityCacheManager a závislosti - viz metody).
/// </summary>
public class DefaultCachingInstaller : ICachingInstaller
{
	/// <inheritdoc />
	public void Install(IServiceCollection services)
	{
		services.TryAddTransient<IEntityCacheManager, EntityCacheManager>();
		RegisterEntityCacheOptionsGenerator(services);
		RegisterEntityCacheKeyGenerator(services);
		RegisterEntityCacheSupportDecision(services);
	}

	/// <summary>
	/// Zaregistruje službu, která vrací CacheOptions, které budou použity pro cachování entit.		
	/// </summary>
	protected virtual void RegisterEntityCacheOptionsGenerator(IServiceCollection services)
	{
		services.TryAddSingleton<IEntityCacheOptionsGenerator, AnnotationsEntityCacheOptionsGenerator>();
		services.TryAddTransient<IAnnotationsEntityCacheOptionsGeneratorStorageBuilder, AnnotationsEntityCacheOptionsGeneratorStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<IAnnotationsEntityCacheOptionsGeneratorStorage>(sp => sp.GetRequiredService<IAnnotationsEntityCacheOptionsGeneratorStorageBuilder>().Build());
	}

	/// <summary>
	/// Zaregistruje službu, která vrací klíče, pod kterými budou položky v cache uloženy.
	/// </summary>
	protected virtual void RegisterEntityCacheKeyGenerator(IServiceCollection services)
	{
		services.TryAddTransient<IEntityCacheKeyGenerator, EntityCacheKeyGenerator>();
	}

	/// <summary>
	/// Zaregistruje službu, která rozhoduje, zda bude daná entita cachována.
	/// </summary>
	protected virtual void RegisterEntityCacheSupportDecision(IServiceCollection services)
	{
		services.TryAddSingleton<IEntityCacheSupportDecision, AnnotationsEntityCacheSupportDecision>();
		services.TryAddTransient<IAnnotationsEntityCacheSupportDecisionStorageBuilder, AnnotationsEntityCacheSupportDecisionStorageBuilder>();
		services.TryAddSingletonFromScopedServiceProvider<IAnnotationsEntityCacheSupportDecisionStorage>(sp => sp.GetRequiredService<IAnnotationsEntityCacheSupportDecisionStorageBuilder>().Build());
	}
}
