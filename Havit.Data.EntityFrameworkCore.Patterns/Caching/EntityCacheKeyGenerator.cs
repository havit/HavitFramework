using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí stringových klíčů do cache.
/// Do klíče generuje názvy typu.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public class EntityCacheKeyGenerator : IEntityCacheKeyGenerator
{
	private readonly IEntityCacheKeyPrefixService _entityCacheKeyPrefixService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheKeyGenerator(IEntityCacheKeyPrefixService entityCacheKeyPrefixService)
	{
		_entityCacheKeyPrefixService = entityCacheKeyPrefixService;
	}

	/// <inheritdoc />
	public string GetEntityCacheKey(Type entityType, object key)
	{
		return _entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + key.ToString();
	}

	/// <inheritdoc />
	public string GetNavigationCacheKey(Type entityType, object key, string propertyName)
	{
		return _entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + key.ToString() + "|" + propertyName;
	}

	/// <inheritdoc />
	public string GetAllKeysCacheKey(Type entityType)
	{
		return _entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "AllKeys";
	}
}
