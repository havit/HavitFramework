using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí stringových klíčů do cache.
/// Do klíče generuje názvy typu.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public class EntityCacheKeyGenerator : IEntityCacheKeyGenerator
{
	private readonly IEntityCacheKeyPrefixService entityCacheKeyPrefixService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyGenerator(IEntityCacheKeyPrefixService entityCacheKeyPrefixService)
	{
		this.entityCacheKeyPrefixService = entityCacheKeyPrefixService;
	}

	/// <inheritdoc />
	public string GetEntityCacheKey(Type entityType, object key)
	{
		return entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + key.ToString();
	}

	/// <inheritdoc />
	public string GetNavigationCacheKey(Type entityType, object key, string propertyName)
	{
		return entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + key.ToString() + "|" + propertyName;
	}

	/// <inheritdoc />
	public string GetAllKeysCacheKey(Type entityType)
	{
		return entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "AllKeys";
	}
}
