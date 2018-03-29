#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Havit.Services.Caching
{
	/// <summary>
	/// Implementace ICacheService pomocí IMemoryCache.
	/// Třída je dostupná pouze pro .NET Standard 2.0 (nikoliv pro .NET Framework).
	/// </summary>
	/// <seealso cref="Havit.Services.Caching.ICacheService" />
    public class MemoryCacheService : ICacheService
    {
	    /// <summary>
	    /// IMemoryCache používaná touto třídou.
	    /// </summary>
	    private readonly IMemoryCache memoryCache;

		/// <summary>
		/// Kostruktor.
		/// </summary>
		/// <param name="memoryCache">IMemoryCache, která bude použita pro cachování.</param>
	    public MemoryCacheService(IMemoryCache memoryCache)
	    {
		    this.memoryCache = memoryCache;
	    }

	    /// <summary>
	    /// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
	    /// Vrací vždy false.
	    /// </summary>
	    public bool SupportsCacheDependencies => false;

	    /// <summary>
	    /// Přidá položku s daným klíčem a hodnotou do cache.
	    /// </summary>
	    public void Add(string key, object value, CacheOptions options = null)
	    {
		    if (options != null)
		    {
			    var cacheEntryOptions = new MemoryCacheEntryOptions();
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
				    throw new InvalidOperationException("Cache Dependencies nejsou podporovány.");
			    }

			    memoryCache.Set(key, value, cacheEntryOptions);
		    }
		    else
		    {
			    memoryCache.Set(key, value);
		    }
	    }

	    /// <summary>
	    /// Vyhledá položku s daným klíčem v cache.
	    /// </summary>
	    /// <returns>
	    /// True, pokud položka v cache je, jinak false.
	    /// </returns>
	    public bool TryGet(string key, out object result)
	    {
		    return memoryCache.TryGetValue(key, out result);
	    }

	    /// <summary>
	    /// Vrací true, pokud je položka s daným klíčem v cache.
	    /// </summary>
	    public bool Contains(string key)
	    {
		    return memoryCache.TryGetValue(key, out object result);
	    }

	    /// <summary>
	    /// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
	    /// </summary>
	    public void Remove(string key)
	    {
		    memoryCache.Remove(key);
	    }

	    /// <summary>
	    /// Vyčistí obsah cache.
	    /// Není podporováno, vyhazuje NotSupportedException.
	    /// V případě potřeby lze doimplementovat.
	    /// </summary>
	    public void Clear()
	    {
			// v případě potřeby můžeme implementovat pomocí CancellationTokenSource
		    // https://stackoverflow.com/questions/34406737/how-to-remove-all-objects-reset-from-imemorycache-in-asp-net-core
		    throw new NotSupportedException();
	    }
    }
}
#endif