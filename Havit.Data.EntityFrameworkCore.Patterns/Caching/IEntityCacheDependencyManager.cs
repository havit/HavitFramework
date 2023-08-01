using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Třída zajišťující invalidaci závislostí v cache.
/// </summary>
public interface IEntityCacheDependencyManager
{
	/// <summary>
	/// Invaliduje závislosti změněných entit.
	/// </summary>
	CacheInvalidationOperation PrepareCacheInvalidation(Changes changes);
}
