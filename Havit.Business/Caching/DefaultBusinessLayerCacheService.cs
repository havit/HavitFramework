using Havit.Data;
using Havit.Services.Caching;

namespace Havit.Business.Caching;

/// <summary>
/// Výchozí implementace cachování business objektů. Všechny objekty jsou cachovány do podkladové ICacheService.
/// Podpora cache dependencies je rovněž určena podkladovou CacheService.
/// </summary>
public class DefaultBusinessLayerCacheService : IBusinessLayerCacheService
{
	private readonly ICacheService cacheService;

	/// <summary>
	/// Indikuje, zda podkladová cache podporuje cache dependencies.
	/// Pokud nepodporuje, BL neposkytne žádný SaveCacheDependencyKey a AnySaveCacheDependencyKey.
	/// Vrací hodnotu podle toho, zda podkladová ICacheService podporuje Cache Dependency.
	/// </summary>
	public bool SupportsCacheDependencies
	{
		get
		{
			return cacheService.SupportsCacheDependencies;
		}
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="cacheService">Instance cacheservice, která zprostředkovává přístup do cache.</param>
	public DefaultBusinessLayerCacheService(ICacheService cacheService)
	{
		this.cacheService = cacheService;
	}

	/// <summary>
	/// Zajistí v cache přítomnost klíče, který je cache dependecy masterem pro závislé položky.
	/// Pokud podkladová CacheService nepodporuje cachování závislostí, vyhazuje výjimku InvalidOperationException.
	/// </summary>
	public void EnsureCacheDependencyKey(Type businessObjectType, string cacheKey)
	{
		if (!SupportsCacheDependencies)
		{
			throw new InvalidOperationException("Cache dependencies nejsou podporovány na použité CacheService.");
		}
		object tmp;
		if (!cacheService.TryGet(cacheKey, out tmp))
		{
			cacheService.Add(cacheKey, new object());
		}
	}

	/// <summary>
	/// Invaliduje v cache klíče, který je cache dependecy masterem.
	/// Pokud podkladová CacheService nepodporuje cachování závislostí, vyhazuje výjimku InvalidOperationException.
	/// </summary>
	public void InvalidateCacheDependencies(Type businessObjectType, string cacheKey)
	{
		if (!SupportsCacheDependencies)
		{
			throw new InvalidOperationException("Cache dependencies nejsou podporovány na použité CacheService.");
		}
		cacheService.Remove(cacheKey);
	}

	/// <summary>
	/// Přidá do cache položku, která je business objektem (cachování readonly objektů).
	/// </summary>
	public void AddBusinessObjectToCache(Type businessObjectType, string cacheKey, BusinessObjectBase businessObject, CacheOptions options = null)
	{
		cacheService.Add(cacheKey, businessObject, options);
	}

	/// <summary>
	/// Vyhledá v cache položku, která je business objektem (cachování readonly objektů).
	/// </summary>
	public BusinessObjectBase GetBusinessObjectFromCache(Type businessObjectType, string cacheKey)
	{
		object result;
		cacheService.TryGet(cacheKey, out result);
		return (BusinessObjectBase)result;
	}

	/// <summary>
	/// Přidá do cache položku, která je DataRecordem (cachování non-readonly objektů).
	/// </summary>
	public void AddDataRecordToCache(Type businessObjectType, string cacheKey, DataRecord dataRecord, CacheOptions options = null)
	{
		cacheService.Add(cacheKey, dataRecord, options);
	}

	/// <summary>
	/// Vyhledá v cache položku, která je DataRecordem (cachování non-readonly objektů).
	/// </summary>
	public DataRecord GetDataRecordFromCache(Type businessObjectType, string cacheKey)
	{
		object result;
		cacheService.TryGet(cacheKey, out result);
		return (DataRecord)result;
	}

	/// <summary>
	/// Odstraní z cache položku, která je DataRecordem (cachování non-readonly objektů).
	/// </summary>
	public void RemoveDataRecordFromCache(Type businessObjectType, string cacheKey)
	{
		cacheService.Remove(cacheKey);
	}

	/// <summary>
	/// Přidá do cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
	/// </summary>
	public void AddAllIDsToCache(Type businessObjectType, string cacheKey, int[] ids, CacheOptions options = null)
	{
		cacheService.Add(cacheKey, ids, options);
	}

	/// <summary>
	/// Vyhledá v cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
	/// </summary>
	public int[] GetAllIDsFromCache(Type businessObjectType, string cacheKey)
	{
		object result;
		cacheService.TryGet(cacheKey, out result);
		return (int[])result;
	}

	/// <summary>
	/// Odstraní z cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
	/// </summary>
	public void RemoveAllIDsFromCache(Type businessObjectType, string cacheKey)
	{
		cacheService.Remove(cacheKey);
	}

	/// <summary>
	/// Přidá do cache položku, která je projekcí databázových resources pro jeden jazyk.
	/// </summary>
	public void AddDbResourcesDataToCache(string cacheKey, object resources, CacheOptions cacheOptions = null)
	{
		cacheService.Add(cacheKey, resources, cacheOptions);
	}

	/// <summary>
	/// Odstraní z cache položku, která je projekcí databázových resources pro jeden jazyk.
	/// </summary>
	public object GetDbResourcesDataFromCache(string cacheKey)
	{
		object result;
		cacheService.TryGet(cacheKey, out result);
		return result;
	}
}
