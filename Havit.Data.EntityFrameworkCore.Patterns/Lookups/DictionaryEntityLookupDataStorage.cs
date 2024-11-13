using System.Collections.Concurrent;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// Určeno pro unit testy.
/// </summary>
public class DictionaryEntityLookupDataStorage : IEntityLookupDataStorage
{
	/// <summary>
	/// Skutečné úložiště lookup data.
	/// </summary>
	private ConcurrentDictionary<object, object> _lookupTables = new ConcurrentDictionary<object, object>();

	/// <inheritdoc />
	public EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey)
	{
		if (_lookupTables.TryGetValue(storageKey, out object result))
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
		_lookupTables.TryAdd(storageKey, entityLookupData);
	}

	/// <inheritdoc />
	public void RemoveEntityLookupData(string storageKey)
	{
		_lookupTables.TryRemove(storageKey, out _);
	}

}
