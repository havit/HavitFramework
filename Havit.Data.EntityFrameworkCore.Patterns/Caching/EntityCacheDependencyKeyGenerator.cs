using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Diagnostics.Contracts;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí strinkových klíčů do cache.
/// </summary>
public class EntityCacheDependencyKeyGenerator : IEntityCacheDependencyKeyGenerator
{
	private readonly ICacheService cacheService;
	private readonly IEntityCacheKeyPrefixService entityCacheKeyPrefixService;
	private static readonly object staticCacheValue = new object();

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public EntityCacheDependencyKeyGenerator(ICacheService cacheService, IEntityCacheKeyPrefixService entityCacheKeyPrefixService)
	{
		this.cacheService = cacheService;
		this.entityCacheKeyPrefixService = entityCacheKeyPrefixService;
	}

	/// <inheritdoc />
	public string GetAnySaveCacheDependencyKey(Type entityType, bool ensureInCache = true)
	{
		EnsureSupportsCacheDependencies();
		string dependencyKey = entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "AnySave";
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
		string dependencyKey = entityCacheKeyPrefixService.GetCacheKeyPrefix(entityType) + "Save|" + key.ToString();
		if (ensureInCache)
		{
			EnsureInCache(dependencyKey);
		}
		return dependencyKey;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureSupportsCacheDependencies()
	{
		Contract.Assert<InvalidOperationException>(cacheService.SupportsCacheDependencies, "Dependency keys can be generated only for ICacheService which supports cache dependencies.");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EnsureInCache(string dependencyKey)
	{
		// musíme se nejprve zeptat, zda exisuje
		// kdybychom se nezeptali a došlo jen k Addu, invalidoval by se dosavadní klíč a vzal by s sebou své závislosti.
		if (!cacheService.Contains(dependencyKey))
		{
			cacheService.Add(dependencyKey, staticCacheValue);
		}
	}
}
