using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;

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
		componentRegistrationOptions.CachingInstaller = new NoCachingInstaller();
		return componentRegistrationOptions;
	}

	/// <summary>
	/// Řekne, že má být zaregistrována služba, která cachuje úplně všechno se sliding expirací.
	/// </summary>
	public static ComponentRegistrationOptions ConfigureCacheAllEntitiesWithDefaultSlidingExpirationCaching(this ComponentRegistrationOptions componentRegistrationOptions, TimeSpan slidingExpiration)
	{
		componentRegistrationOptions.CachingInstaller = new CacheAllEntitiesWithDefaultSlidingExpirationCachingInstaller(slidingExpiration);
		return componentRegistrationOptions;
	}

	/// <summary>
	/// Řekne, že má být zaregistrována služba vlastním installerem.
	/// </summary>
	public static ComponentRegistrationOptions ConfigureCustomCaching(this ComponentRegistrationOptions componentRegistrationOptions, ICachingInstaller installer)
	{
		componentRegistrationOptions.CachingInstaller = installer;
		return componentRegistrationOptions;
	}
}
