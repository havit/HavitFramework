#if NET46
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Havit.Services.Caching
{
	/// <summary>
	/// Implementace ICacheService pomocí ObjectCache.
	/// Třída je dostupná pouze pro full .NET Framework (nikoliv pro .NET Standard 2.0).
	/// </summary>
	/// <seealso cref="Havit.Services.Caching.ICacheService" />
	public class ObjectCacheService : ICacheService
	{
		/// <summary>
		/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
		/// </summary>
		public bool SupportsCacheDependencies { get; }

		/// <summary>
		/// ObjectCache používaná touto třídou.
		/// </summary>
		public ObjectCache CurrentObjectCache { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="objectCache">Object cache, která bude použita pro cachování.</param>
		/// <param name="objectCacheSupportsCacheDependencies">Indikuje, zda předaná object cache podporuje cache dependencies.</param>
		public ObjectCacheService(ObjectCache objectCache, bool objectCacheSupportsCacheDependencies)
		{
			this.CurrentObjectCache = objectCache;
			this.SupportsCacheDependencies = objectCacheSupportsCacheDependencies;
		}

		/// <summary>
		/// Přidá položku s daným klíčem a hodnotou do cache.
		/// Prioritu NotRemovable respektuje, pro všechny ostatní použije Default (omezení ObjectCache).
		/// </summary>
		public virtual void Add(string key, object value, CacheOptions options = null)
		{
			CacheItemPolicy cacheItemPolicy = null;

			if (options != null)
			{
				cacheItemPolicy = new CacheItemPolicy();

				if (options.AbsoluteExpiration != null)
				{
					cacheItemPolicy.AbsoluteExpiration = DateTimeOffset.Now.Add(options.AbsoluteExpiration.Value);
				}

				if (options.SlidingExpiration != null)
				{
					cacheItemPolicy.SlidingExpiration = options.SlidingExpiration.Value;
				}

				if (options.Priority == CacheItemPriority.NotRemovable)
				{
					cacheItemPolicy.Priority = System.Runtime.Caching.CacheItemPriority.NotRemovable;
				}

				if ((options.CacheDependencyKeys != null) && (options.CacheDependencyKeys.Length > 0))
				{
					if (!SupportsCacheDependencies)
					{
						throw new InvalidOperationException("Cache Dependencies nejsou podporovány.");
					}
					cacheItemPolicy.ChangeMonitors.Add(CurrentObjectCache.CreateCacheEntryChangeMonitor(options.CacheDependencyKeys));
				}
			}

			CurrentObjectCache.Add(key, value, cacheItemPolicy);
		}

		/// <summary>
		/// Vyhledá položku s daným klíčem v cache.
		/// </summary>
		/// <returns>
		/// True, pokud položka v cache je, jinak false.
		/// </returns>
		public virtual bool TryGet(string key, out object result)
		{
			result = CurrentObjectCache.Get(key);
			return (result != null);
		}

		/// <summary>
		/// Vrací true, pokud je položka s daným klíčem v cache.
		/// </summary>
		public virtual bool Contains(string key)
		{
			return CurrentObjectCache.Contains(key);
		}

		/// <summary>
		/// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
		/// </summary>
		public virtual void Remove(string key)
		{
			CurrentObjectCache.Remove(key);
		}

		/// <summary>
		/// Vyčistí obsah cache.
		/// Odstraňuje obsah položku po položce.
		/// </summary>
		public virtual void Clear()
		{
			List<string> keys = CurrentObjectCache.Select(kvp => kvp.Key).ToList();
			foreach (string key in keys)
			{
				Remove(key);
			}
		}
	}
}
#endif