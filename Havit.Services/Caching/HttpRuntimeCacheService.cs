#if NET46
using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Havit.Services.Caching
{
	/// <summary>
	/// Implementace ICacheService s využitím cache "HttpRuntime.Cache".
	/// Třída je dostupná pouze pro full .NET Framework (nikoliv pro .NET Standard 2.0).
	/// </summary>
	/// <seealso cref="Havit.Services.Caching.ICacheService" />
	public class HttpRuntimeCacheService : ICacheService
	{
		private readonly System.Web.Caching.Cache cache;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public HttpRuntimeCacheService()
		{
			cache = HttpRuntime.Cache;
		}

		/// <summary>
		/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
		/// Tato třída vrací vždy true.
		/// </summary>
		public bool SupportsCacheDependencies
		{
			get
			{
				return true;				
			}
		}

		/// <summary>
		/// Přidá položku s daným klíčem a hodnotou do cache.
		/// </summary>
		public void Add(string key, object value, CacheOptions options)
		{
			CacheDependency cacheDependency = null;
			DateTime absoluteExpiration = Cache.NoAbsoluteExpiration;
			TimeSpan slidingExpiration = Cache.NoSlidingExpiration;
			System.Web.Caching.CacheItemPriority priority = System.Web.Caching.CacheItemPriority.Default;

			if (options != null)
			{
				if (options.CacheDependencyKeys != null)
				{
					cacheDependency = new CacheDependency(null, options.CacheDependencyKeys);
				}

				if (options.AbsoluteExpiration != null)
				{
					absoluteExpiration = (options.AbsoluteExpiration != null) ? DateTime.Now.Add(options.AbsoluteExpiration.Value) : Cache.NoAbsoluteExpiration;
				}

				if (options.SlidingExpiration != null)
				{
					slidingExpiration = options.SlidingExpiration.Value;
				}

				switch (options.Priority)
				{
					case CacheItemPriority.Low:
						priority = System.Web.Caching.CacheItemPriority.Low;
						break;

					case CacheItemPriority.Normal:
						priority = System.Web.Caching.CacheItemPriority.Normal;
						break;

					case CacheItemPriority.High:
						priority = System.Web.Caching.CacheItemPriority.High;
						break;

					case CacheItemPriority.NotRemovable:
						priority = System.Web.Caching.CacheItemPriority.NotRemovable;
						break;

					default:
						throw new NotSupportedException(String.Format("Hodnota CacheItemPriority.{0} není podporována.", options.Priority.ToString()));
				}
			}

			cache.Insert(
				key,
				value,
				cacheDependency,
				absoluteExpiration,
				slidingExpiration,
				priority,
				null // callback
			);
		}

		/// <summary>
		/// Vyhledá položku s daným klíčem v cache.
		/// </summary>
		/// <returns>
		/// True, pokud položka v cache je, jinak false.
		/// </returns>
		public bool TryGet(string key, out object result)
		{
			result = cache.Get(key);
			return (result != null);
		}

		/// <summary>
		/// Vrací true, pokud je položka s daným klíčem v cache.
		/// </summary>
		public bool Contains(string key)
		{
			return cache.Get(key) != null;
		}

		/// <summary>
		/// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
		/// </summary>
		public void Remove(string key)
		{
			cache.Remove(key);
		}

		/// <summary>
		/// Vyčistí obsah cache, kontrétně celé HttpRuntime.Cache.
		/// </summary>
		public void Clear()
		{
			foreach (DictionaryEntry dictionaryEntry in cache)
			{
				cache.Remove((string)dictionaryEntry.Key);
			}
		}
	}
}
#endif