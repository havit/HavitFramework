using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí strinkových klíčů do cache.
/// </summary>
public class EntityCacheDependencyKeyGenerator : IEntityCacheDependencyKeyGenerator
{
	private readonly ICacheService _cacheService;
	private readonly IEntityCacheKeyPrefixService _entityCacheKeyPrefixService;
	private static readonly object _staticCacheValue = new object();

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheDependencyKeyGenerator(ICacheService cacheService, IEntityCacheKeyPrefixService entityCacheKeyPrefixService)
	{
		_cacheService = cacheService;
		_entityCacheKeyPrefixService = entityCacheKeyPrefixService;
	}

	/// <inheritdoc />
	public string GetAnySaveCacheDependencyKey(Type entityType, bool ensureInCache = true)
	{
		EnsureSupportsCacheDependencies();
		string dependencyKey = _entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "AnySave";
		if (ensureInCache)
		{
			EnsureInCache(dependencyKey);
		}
		return dependencyKey;
	}

	/// <inheritdoc />
	public string GetSaveCacheDependencyKey(Type entityType, object key, bool ensureInCache = true)
	{
		EnsureSupportsCacheDependencies();
		string dependencyKey = _entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "Save|" + key.ToString();
		if (ensureInCache)
		{
			EnsureInCache(dependencyKey);
		}
		return dependencyKey;
	}

	private void EnsureSupportsCacheDependencies()
	{
		Contract.Assert<InvalidOperationException>(_cacheService.SupportsCacheDependencies, "Dependency keys can be generated only for ICacheService which supports cache dependencies.");
	}

	private void EnsureInCache(string dependencyKey)
	{
		// musíme se nejprve zeptat, zda exisuje
		// kdybychom se nezeptali a došlo jen k Addu, invalidoval by se dosavadní klíč a vzal by s sebou své závislosti.

		if (!_cacheService.Contains(dependencyKey))
		{
			_cacheService.Add(dependencyKey, _staticCacheValue);
		}
	}
}
