using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Services.Caching;
using StackExchange.Redis;

namespace Havit.Services.Redis.Caching
{
	/// <summary>
	/// Distribuovaná cache.
	/// Předpokládá data v lokálních cache. Zajišťuje invalidaci ostatních instancí aplikace pomocí RedisCache a jeho zpráv (RedisChannel).
	/// Předpokládá použití jako singleton.
	/// </summary>
	public class DistributedRedisInvalidatingCache : Havit.Services.Caching.ICacheService
	{
		private readonly ICacheService localCacheService;
		private readonly RedisChannel redisClearChannel;
		private readonly RedisChannel redisRemoveChannel;
		private readonly ISubscriber redisSubscriber;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DistributedRedisInvalidatingCache(string redisConnectionString, string redisChannelNamePrefix, ICacheService localCacheService)
		{
			this.localCacheService = localCacheService;

			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);

			redisClearChannel = new RedisChannel(redisChannelNamePrefix + "/message/Clear", RedisChannel.PatternMode.Literal);
			redisRemoveChannel = new RedisChannel(redisChannelNamePrefix + "/message/Remove", RedisChannel.PatternMode.Literal);
			redisSubscriber = redis.GetSubscriber();

			// při příjmu zprávy o odstranění poloýky z cache odstraníme příslušný klíč z lokální cache
			redisSubscriber.Subscribe(redisRemoveChannel, (channel, message) => localCacheService.Remove(message));

			// při příjmu zprávy o smazání cache odstraníme vše z lokální cache
			redisSubscriber.Subscribe(redisClearChannel, (channel, message) => localCacheService.Clear());
		}

		/// <summary>
		/// Indikuje, zda cache podporuje cache dependencies, tj. mechanismus, kdy při výpadku určitého klíče z cache má být odstraněn i jiný klíč. Vrací vždy false.
		/// </summary>
		public bool SupportsCacheDependencies => false;

		/// <summary>
		/// Přidá položku s daným klíčem a hodnotou do lokální cache.
		/// </summary>
		public void Add(string key, object value, CacheOptions options = null)
		{
			localCacheService.Add(key, value, options);
		}

		/// <summary>
		/// Vyčistí obsah lokální cache.
		/// Dá vědět ostatním instancím aplikace informaci o nutnosti odstranění položky z cache.
		/// </summary>
		public void Clear()
		{
			redisSubscriber.PublishAsync(redisClearChannel, String.Empty, CommandFlags.FireAndForget);
			localCacheService.Clear();
		}

		/// <summary>
		/// Vrací true, pokud je položka s daným klíčem v lokální cache.
		/// </summary>
		public bool Contains(string key)
		{
			return localCacheService.Contains(key);
		}

		/// <summary>
		/// Odstraní položku s daným klíčem z lokální cache. Pokud položka v lokální cache není, nic neudělá.
		/// Dá vědět ostatním instancím aplikace informaci o nutnosti odstranění položky z cache.
		/// </summary>
		public void Remove(string key)
		{
			redisSubscriber.PublishAsync(redisRemoveChannel, key, CommandFlags.FireAndForget);
			localCacheService.Remove(key);
		}

		/// <summary>
		/// Vyhledá položku s daným klíčem v lokální cache.
		/// </summary>
		/// <returns>True, pokud položka v lokální cache je, jinak false.</returns>
		public bool TryGet(string key, out object result)
		{
			return localCacheService.TryGet(key, out result);
		}
	}
}
