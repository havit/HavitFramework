using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Cache manager, který nic necachuje a nic nečte z cache.
/// Nemá žádné závislosti.
/// </summary>
public sealed class NoCachingEntityCacheManager : IEntityCacheManager
{
	/// <summary>
	/// Vrací vždy false.
	/// </summary>
	public bool ShouldCacheEntityType<TEntity>()
	{
		return false;
	}

	/// <summary>
	/// Nic nedělá, nehledá v cache.
	/// Vrací vždy false.
	/// </summary>
	public bool TryGetEntity<TEntity>(object id, out TEntity entity)
		where TEntity : class
	{
		entity = null;
		return false;
	}

	/// <summary>
	/// Nic nedělá, neukládá do cache.
	/// </summary>
	public void StoreEntity<TEntity>(TEntity entity)
		where TEntity : class
	{
		// NOOP
	}

	/// <summary>
	/// Nic nedělá, nehledá v cache.
	/// Vrací vždy false.
	/// </summary>
	public bool TryGetNavigation<TEntity, TPropertyItem>(TEntity entityToLoad, string propertyName)
		where TEntity : class
		where TPropertyItem : class
	{
		return false;
	}

	/// <summary>
	/// Nic nedělá, neukládá do cache.
	/// </summary>
	public void StoreNavigation<TEntity, TPropertyItem>(TEntity entity, string propertyName)
		where TEntity : class
		where TPropertyItem : class
	{
		// NOOP
	}

	/// <summary>
	/// Nic nedělá, nehledá v cache.
	/// Vrací vždy false.
	/// </summary>
	public bool TryGetAllKeys<TEntity>(out object keys)
		where TEntity : class
	{
		keys = null;
		return false;
	}

	/// <summary>
	/// Nic nedělá, neukládá do cache.
	/// </summary>
	public void StoreAllKeys<TEntity>(Func<object> keysFunc)
		where TEntity : class
	{
		// NOOP
	}

	/// <summary>
	/// Nic nedělá, neinvaliduje (vrací vždy null).	
	/// </summary>
	public CacheInvalidationOperation PrepareCacheInvalidation(Changes changes)
	{
		return null;
	}
}
