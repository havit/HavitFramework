using System.Collections.Concurrent;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// </summary>
public class EntityLookupDataStorage : IEntityLookupDataStorage
{
	/// <summary>
	/// Skutečné úložiště lookup data.
	/// </summary>
	private ConcurrentDictionary<object, object> lookupTables = new ConcurrentDictionary<object, object>();

	/// <inheritdoc />
	public EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey, Func<EntityLookupData<TEntity, TEntityKey, TLookupKey>> factory)
	{
		if (factory == null)
		{
			if (lookupTables.TryGetValue(storageKey, out object result))
			{
				// nemáme factory, ale našli jsme položku v dictionary
				return (EntityLookupData<TEntity, TEntityKey, TLookupKey>)result;
			}
			else
			{
				// nemáme factory, nemáme položku v dictionary
				return null;
			}
		}
		else
		{
			// máme factory
			return (EntityLookupData<TEntity, TEntityKey, TLookupKey>)lookupTables.GetOrAdd(storageKey, _ => factory());
		}
	}

	/// <inheritdoc />
	public void RemoveEntityLookupData(string storageKey)
	{
		lookupTables.TryRemove(storageKey, out _);
	}
}
