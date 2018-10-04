#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Havit.Services.Caching
{
	/// <summary>
	/// Implementace ICacheService pomocí IMemoryCache s podporou cache dependencies.
	/// Třída je dostupná pouze pro .NET Standard 2.0 (nikoliv pro .NET Framework).
	/// Pozor na paměťovou náročnost, infrastruktura cachování potřebuje okolo 500 bytes na záznam.
	/// </summary>
	/// <seealso cref="Havit.Services.Caching.ICacheService" />
    public class MemoryCacheSupportingDependenciesService : ICacheService
    {
	    /// <summary>
	    /// IMemoryCache používaná touto třídou.
	    /// </summary>
	    private readonly IMemoryCache memoryCache;
		
		/// <summary>
		/// Kostruktor.
		/// </summary>
		/// <param name="memoryCache">IMemoryCache, která bude použita pro cachování.</param>
		public MemoryCacheSupportingDependenciesService(IMemoryCache memoryCache)
	    {
		    this.memoryCache = memoryCache;
	    }

	    /// <summary>
	    /// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč.
	    /// Vrací vždy false.
	    /// </summary>
	    public bool SupportsCacheDependencies => true;

	    /// <summary>
	    /// Přidá položku s daným klíčem a hodnotou do cache.
	    /// </summary>
	    public void Add(string key, object value, CacheOptions options = null)
	    {
			var cacheEntryOptions = new MemoryCacheEntryOptions();

		    if (options != null)
		    {
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
			}

			// kdykoliv je vyhozena položka z cache, cancelujeme její token, čímž dojde k vyhození závislých položek
			cacheEntryOptions = cacheEntryOptions.RegisterPostEvictionCallback(RemoveDependenyEntriesWhenEvicted);

			memoryCache.Set(key, new CacheEntry(value, new CancellationTokenSource()), cacheEntryOptions);
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
		    bool result = memoryCache.TryGetValue(key, out CacheEntry cacheEntry);
			value = result ? cacheEntry.Value : null;
			return result;
	    }

	    /// <summary>
	    /// Vrací true, pokud je položka s daným klíčem v cache.
	    /// </summary>
	    public bool Contains(string key)
	    {
		    return memoryCache.TryGetValue(key, out CacheEntry cacheEntry);
	    }

	    /// <summary>
	    /// Odstraní položku s daným klíčem z cache. Pokud položka v cache není, nic neudělá.
	    /// </summary>
	    public void Remove(string key)
	    {
			if (memoryCache.TryGetValue(key, out CacheEntry cacheEntry))
			{
				memoryCache.Remove(key);
				// přímé závislosti jsou z cache vyhozeny "později", nikoliv hned v rámci remove (ve vyhrazeném threadu?)
				// pokud je chceme vyhodit okamžitě, lze takto
				((CacheEntry)cacheEntry).CancellationTokenSource.Cancel();
			}		    
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
}
#endif