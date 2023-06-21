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
		services.TryAddTransient<IEntityCacheOptionsGenerator, AnnotationsEntityCacheOptionsGenerator>();
		services.TryAddSingleton<IAnnotationsEntityCacheOptionsGeneratorStorage, AnnotationsEntityCacheOptionsGeneratorStorage>();
	}

	/// <summary>
	/// Zaregistruje službu, která vrací klíče, pod kterými budou položky v cache uloženy.
	/// </summary>
	protected virtual void RegisterEntityCacheKeyGenerator(IServiceCollection services)
	{
		services.TryAddTransient<IEntityCacheKeyGenerator, EntityCacheKeyGenerator>();
		services.TryAddSingleton<IEntityCacheKeyGeneratorStorage, EntityCacheKeyGeneratorStorage>();
	}

	/// <summary>
	/// Zaregistruje službu, která rozhoduje, zda bude daná entita cachována.
	/// </summary>
	protected virtual void RegisterEntityCacheSupportDecision(IServiceCollection services)
	{
		services.TryAddTransient<IEntityCacheSupportDecision, AnnotationsEntityCacheSupportDecision>();
		services.TryAddSingleton<IAnnotationsEntityCacheSupportDecisionStorage, AnnotationsEntityCacheSupportDecisionStorage>();
	}
}
