using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Cache manager, který nic neinvaliduje.
/// Nemá žádné závislosti.
/// </summary>
public class NoCachingEntityCacheDependencyManager : IEntityCacheDependencyManager
{
	/// <inheritdoc />
	public CacheInvalidationOperation PrepareCacheInvalidation(Changes changes)
	{
		return null;
	}
}
