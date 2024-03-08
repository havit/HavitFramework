using System.Collections.Concurrent;
using Havit.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Lookups;

/// <summary>
/// Úložiště lookup dat.
/// </summary>
public class EntityLookupDataStorage : IEntityLookupDataStorage
{
	/// <summary>
	/// Skutečné úložiště lookup data.
	/// </summary>
	private ConcurrentDictionary<object, object> _lookupTables = new ConcurrentDictionary<object, object>();
	private CriticalSection<string> _criticalSection = new CriticalSection<string>();

	/// <inheritdoc />
	public EntityLookupData<TEntity, TEntityKey, TLookupKey> GetEntityLookupData<TEntity, TEntityKey, TLookupKey>(string storageKey, Func<EntityLookupData<TEntity, TEntityKey, TLookupKey>> factory)
	{
		if (factory == null)
		{
			if (_lookupTables.TryGetValue(storageKey, out object result))
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
			return (EntityLookupData<TEntity, TEntityKey, TLookupKey>)_lookupTables.GetOrAdd(storageKey, _ => factory());
		}
	}

	/// <inheritdoc />
	public async ValueTask<EntityLookupData<TEntity, TEntityKey, TLookupKey>> GetEntityLookupDataAsync<TEntity, TEntityKey, TLookupKey>(string storageKey, Func<CancellationToken, Task<EntityLookupData<TEntity, TEntityKey, TLookupKey>>> factory, CancellationToken cancellationToken)
	{
		if (factory == null)
		{
			if (_lookupTables.TryGetValue(storageKey, out object result))
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

			// Nepoužjeme GetOrAdd, protože factory pro add nemůže být "asynchronní", ale my máme "asynchronní".
			// Pokud již máme data v cache, nepotřebujeme vrátíme data bez použití factory.
			if (_lookupTables.TryGetValue(storageKey, out var lookupTableResult))
			{
				return (EntityLookupData<TEntity, TEntityKey, TLookupKey>)lookupTableResult;
			}

			// Musíme získat data z factory, nicméně chceme zajistit, aby se data pro jeden storage nenačítala paralelně.
			// Uvnitř sekce lock { } nelze použít await, proto používám řešení s CriticalSection.
			EntityLookupData<TEntity, TEntityKey, TLookupKey> result = null;
			await _criticalSection.ExecuteActionAsync(storageKey, async () =>
			{
				// double checking pattern
				if (_lookupTables.TryGetValue(storageKey, out var lookupTableResult))
				{
					// Pokud jsme druzí, kdo vstoupil do kritické sekce, máme již data v lookupTables.
					result = (EntityLookupData<TEntity, TEntityKey, TLookupKey>)lookupTableResult;
				}
				else
				{
					// Pokud jsme první, kdo vstoupil do kritické sekce, načteme data a uložíme je do lookupTables.
					result = await factory(cancellationToken).ConfigureAwait(false);
					_lookupTables.TryAdd(storageKey, result);
				}
			}, cancellationToken).ConfigureAwait(false);
			return result;
		}
	}


	/// <inheritdoc />
	public void RemoveEntityLookupData(string storageKey)
	{
		_lookupTables.TryRemove(storageKey, out _);
	}
}
