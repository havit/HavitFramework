﻿using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Havit.Services.Caching;

/// <summary>
/// Implementace ICacheService pomocí IMemoryCache s volitelnou podporou cache dependencies.
/// Třída je dostupná pouze pro .NET Standard 2.0 (nikoliv pro .NET Framework).
/// Pozor na paměťovou náročnost, infrastruktura cachování se závuslostmi potřebuje okolo 500 bytes na záznam.
/// </summary>
/// <seealso cref="Havit.Services.Caching.ICacheService" />
/// <remarks>
/// V situaci, kdy má třída dva konstruktory
/// - konstruktor s IMemoryCache a bool s výchozí hodnotou
/// - konstruktor s IMemoryCache a MemoryCacheServiceOptions
/// si Microsoftí service provider neporadí (Exception), který konstruktor použít. 
/// Při použití MemoryCacheServiceOptions si service provider myslí, že umí resolvovat oba konstuktory, oba se stejným počtem parametrů (2), protože počítá, že umí i bool s výchozí hodnotou.
/// 
/// Proto raději použijeme tři konstruktory, kde si již Microsoftí service provider se situací poradí.
/// - konstruktor jen s IMemoryCache
/// - konstruktor s IMemoryCache a bool
/// - konstruktor s IMemoryCache a MemoryCacheServiceOptions
/// </remarks>
public class MemoryCacheService : ICacheService
{
	/// <summary>
	/// IMemoryCache používaná touto třídou.
	/// </summary>
	private readonly IMemoryCache memoryCache;

	/// <summary>
	/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
	/// </summary>
	public bool SupportsCacheDependencies { get; }

	/// <summary>
	/// Konstruktor.
	/// Nebude použita podpora pro cache dependencies.
	/// </summary>
	/// <param name="memoryCache">IMemoryCache, která bude použita pro cachování.</param>		
	public MemoryCacheService(IMemoryCache memoryCache) : this(memoryCache, false)
	{
		// NOOP
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="memoryCache">IMemoryCache, která bude použita pro cachování.</param>
	/// <param name="useCacheDependenciesSupport">Indikuje, zda má být použita podpora pro cache dependencies.</param>
	public MemoryCacheService(IMemoryCache memoryCache, bool useCacheDependenciesSupport)
	{
		this.memoryCache = memoryCache;
		this.SupportsCacheDependencies = useCacheDependenciesSupport;
	}

	/// <summary>
	/// Konstruktor (DI friendly).
	/// </summary>
	/// <param name="memoryCache">IMemoryCache, která bude použita pro cachování.</param>
	/// <param name="options">Indikuje, zda má být použita podpora pro cache dependencies.</param>
	public MemoryCacheService(IMemoryCache memoryCache, MemoryCacheServiceOptions options) : this(memoryCache, options.UseCacheDependenciesSupport)
	{
		// NOOP
	}

	/// <summary>
	/// Přidá položku s daným klíčem a hodnotou do cache.
	/// </summary>
	public void Add(string key, object value, CacheOptions options = null)
	{
		MemoryCacheEntryOptions cacheEntryOptions = null;

		if (SupportsCacheDependencies)
		{
			cacheEntryOptions = new MemoryCacheEntryOptions();
			// kdykoliv je vyhozena položka z cache, cancelujeme její token, čímž dojde k vyhození závislých položek
			cacheEntryOptions = cacheEntryOptions.RegisterPostEvictionCallback(RemoveDependenyEntriesWhenEvicted);
		}

		if (options != null)
		{
			if (!SupportsCacheDependencies)
			{
				// pokud podporujeme cache dependencies, jsou iž cacheEntryOptions založeny
				cacheEntryOptions = new MemoryCacheEntryOptions();
			}
			if (options.AbsoluteExpiration != null)
			{
				cacheEntryOptions = cacheEntryOptions.SetAbsoluteExpiration(DateTimeOffset.Now.Add(options.AbsoluteExpiration.Value));
			}

			if (options.SlidingExpiration != null)
			{
				cacheEntryOptions = cacheEntryOptions.SetSlidingExpiration(options.SlidingExpiration.Value);
			}

			switch (options.Priority)
			{
				case CacheItemPriority.Low:
					cacheEntryOptions = cacheEntryOptions.SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.Low);
					break;

				case CacheItemPriority.Normal:
					cacheEntryOptions = cacheEntryOptions.SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.Normal);
					break;

				case CacheItemPriority.High:
					cacheEntryOptions = cacheEntryOptions.SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.High);
					break;

				case CacheItemPriority.NotRemovable:
					cacheEntryOptions = cacheEntryOptions.SetPriority(Microsoft.Extensions.Caching.Memory.CacheItemPriority.NeverRemove);
					break;

				default:
					throw new NotSupportedException(String.Format("Hodnota CacheItemPriority.{0} není podporována.", options.Priority.ToString()));
			}

			if ((options.CacheDependencyKeys != null) && (options.CacheDependencyKeys.Length > 0))
			{
				if (!SupportsCacheDependencies)
				{
					throw new InvalidOperationException("Cache Dependencies nejsou podporovány.");
				}

				foreach (string dependencyKey in options.CacheDependencyKeys)
				{
					CancellationToken cancellationToken = GetCancellationToken(dependencyKey);

					if (cancellationToken == CancellationToken.None)
					{
						// token nemáme, pokud položka na které máme být závislý není v cache
						// proto ani svoji položku do cache nedáváme --> končíme metodu
						return;
					}

					cacheEntryOptions = cacheEntryOptions.AddExpirationToken(new CancellationChangeToken(cancellationToken));
				}
			}

			if (options.Size != null)
			{
				cacheEntryOptions = cacheEntryOptions.SetSize(options.Size.Value);
			}
		}

		memoryCache.Set(key, SupportsCacheDependencies ? new CacheEntry(value, new CancellationTokenSource()) : value, cacheEntryOptions);
	}

	private CancellationToken GetCancellationToken(string dependencyKey)
	{
		return memoryCache.TryGetValue(dependencyKey, out CacheEntry cacheEntry)
			? cacheEntry.CancellationTokenSource.Token
			: CancellationToken.None;
	}

	/// <summary>
	/// Vyhledá položku s daným klíčem v cache.
	/// </summary>
	/// <returns>
	/// True, pokud položka v cache je, jinak false.
	/// </returns>
	public bool TryGet(string key, out object value)
	{
		bool result = memoryCache.TryGetValue(key, out object cacheValue);
		value = result
			? (SupportsCacheDependencies ? ((CacheEntry)cacheValue).Value : cacheValue)
			: null;
		return result;
	}

	/// <summary>
	/// Vrací true, pokud je položka s daným klíčem v cache.
	/// </summary>
	public bool Contains(string key)
	{
		return memoryCache.TryGetValue(key, out object _);
	}

	/// <summary>
	/// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
	/// </summary>
	public void Remove(string key)
	{
		if (memoryCache.TryGetValue(key, out object cacheValue))
		{
			memoryCache.Remove(key);
			// přímé závislosti jsou z cache vyhozeny "později", nikoliv hned v rámci remove (ve vyhrazeném threadu?)
			// pokud je chceme vyhodit okamžitě, lze takto
			if (SupportsCacheDependencies)
			{
				((CacheEntry)cacheValue).CancellationTokenSource.Cancel();
			}
		}
	}

	/// <summary>
	/// Vyčistí obsah cache.
	/// </summary>
	public void Clear()
	{
		// Řešení pro MemoryCache od .NET 7

		// Od .NET 7 má MemoryCache (avšak nikoliv IMemoryCache) metodu Clear
		MethodInfo clearMethod = memoryCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
		if (clearMethod != null)
		{
			clearMethod.Invoke(memoryCache, null);
			return;
		}

		// Řešení pro MemoryCache před .NET 7

		// Nejvíce doporučené řešení s CancellationTokenSource na následujícím odkazu nemůžeme použít, protože do IMemoryCache se dostanou objekty i mimo naše metody.
		// My chceme vyčistit všechno, ať už se dostalo do IMemoryCache jakkoliv, proto volíme řešení s reflexí (stabilitu ověříme/ochráníme unit testem).
		// https://stackoverflow.com/questions/34406737/how-to-remove-all-objects-reset-from-imemorycache-in-asp-net-core

		PropertyInfo entriesCollectionPropertyInfo = memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
		if (entriesCollectionPropertyInfo != null)
		{
			object cacheEntriesCollection = entriesCollectionPropertyInfo.GetValue(memoryCache);
			MethodInfo cacheEntriesClearMethod = cacheEntriesCollection.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
			if (cacheEntriesClearMethod == null)
			{
				throw new NotSupportedException("IMemoryCache.EntriesCollection does not have a Clear() method.");
			}
			cacheEntriesClearMethod.Invoke(cacheEntriesCollection, null);
			return;
		}

		throw new NotSupportedException("IMemoryCache implementation has neither Clear method nor EntriesCollection property.");
	}

	/// <summary>
	/// Metoda je volána při vyhození jakékoliv položky z cache (je registrována pro každou položku).
	/// Zajistí cancelování tokenu, který položka nese s sebou.
	/// Tím dojde k vyhození i závislých položek.
	/// </summary>
	private void RemoveDependenyEntriesWhenEvicted(object key, object value, EvictionReason reason, object state)
	{
		((CacheEntry)value).CancellationTokenSource.Cancel();
	}

	internal class CacheEntry
	{
		public object Value { get; }
		public CancellationTokenSource CancellationTokenSource { get; }

		public CacheEntry(object value, CancellationTokenSource cancellationTokenSource)
		{
			Value = value;
			CancellationTokenSource = cancellationTokenSource;
		}
	}
}
