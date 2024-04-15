using System.Runtime.CompilerServices;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// Data se ukládají do ICacheService, čímž zajistíme, že operace "ClearCache" zafunguje i na lookup services.
/// </summary>
public class CacheEntityLookupDataStorage(
	ICacheService _cacheService) : IEntityLookupDataStorage
{
	/// <summary>
	/// Vrátí klíč, pod kterým bude storage uložen v cache.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected virtual string GetCacheKey(string storageKey) => storageKey;

	/// <inheritdoc />
	public EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey)
	{
		if (_cacheService.TryGet(GetCacheKey(storageKey), out object result))
		{
			return (EntityLookupData<TEntity, TEntityKey, TLookupKey>)result;
		}
		else
		{
			return null;
		}
	}

	/// <inheritdoc />
	public void StoreEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey, EntityLookupData<TEntity, TEntityKey, TLookupKey> entityLookupData)
	{
		_cacheService.Add(GetCacheKey(storageKey), entityLookupData);
	}

	/// <inheritdoc />
	public void RemoveEntityLookupData(string storageKey)
	{
		_cacheService.Remove(GetCacheKey(storageKey));
	}
}
