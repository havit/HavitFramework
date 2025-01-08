using Havit.Services.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Services.SignalR.Caching;

/// <summary>
/// Implementace <see cref="ICacheService" />, která deleguje volání na _localCacheService a zároveň zaznamenává požadavky na distribuovanou invalidaci cache.
/// Předpokládá se registrace <see cref="ICacheService"/> zajišťující práci s lokální cache do DI containeru pod klíč <see cref="LocalCacheServiceKey"/>.
/// </summary>
/// <param name="_localCacheService">Cache service pracující s lokální cache.</param>
/// <param name="_distributedInvalidationStorageService">Služba obsluhující úložiště požadavů na distribuovanou invalidaci.</param>
public class DistributedCacheInvalidationCacheService(
	[FromKeyedServices(DistributedCacheInvalidationCacheService.LocalCacheServiceKey)] ICacheService _localCacheService,
	IDistributedCacheInvalidationStorageService _distributedInvalidationStorageService)
	: ICacheService
{
	/// <summary>
	/// Klíč pro pro registraci do DI containeru ICacheService zajišťující lokální cachování.
	/// </summary>
	public const string LocalCacheServiceKey = "LocalCache";

	/// <inheritdoc />
	public bool SupportsCacheDependencies => _localCacheService.SupportsCacheDependencies;

	/// <inheritdoc />
	public void Add(string key, object value, CacheOptions options = null)
	{
		_localCacheService.Add(key, value, options);
	}

	/// <inheritdoc />
	public void Clear()
	{
		_localCacheService.Clear();
		_distributedInvalidationStorageService.SetClearCacheRequired();
	}

	/// <inheritdoc />
	public bool Contains(string key) => _localCacheService.Contains(key);

	/// <inheritdoc />
	public void Remove(string key)
	{
		_localCacheService.Remove(key);
		_distributedInvalidationStorageService.AddKeyToRemove(key);
	}

	/// <inheritdoc />
	public bool TryGet(string key, out object result) => _localCacheService.TryGet(key, out result);
}
