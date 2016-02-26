using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data;
using Havit.Services.Caching;

namespace Havit.Business.Caching
{
	/// <summary>
	/// Služba zajišťující cachování dat používaných business layerem.
	/// </summary>
	public interface IBusinessLayerCacheService
	{
		/// <summary>
		/// Indikuje, zda podkladová cache podporuje cache dependencies.
		/// Pokud nepodporuje, BL neposkytne žádný SaveCacheDependencyKey a AnySaveCacheDependencyKey.
		/// </summary>
		/// <remarks>
		/// Metody přijímají typ business objektu pro možnost selektivního řešení cachování.
		/// </remarks>
		bool SupportsCacheDependencies { get; }

		/// <summary>
		/// Zajistí v cache přítomnost klíče, který je cache dependecy masterem pro závislé položky.
		/// </summary>
		void EnsureCacheDependencyKey(Type businessObjectType, string key);

		/// <summary>
		/// Invaliduje v cache klíče, který je cache dependecy masterem.
		/// </summary>
		void InvalidateCacheDependencies(Type businessObjectType, string key);

		/// <summary>
		/// Přidá do cache položku, která je business objektem (cachování readonly objektů).
		/// </summary>
		void AddBusinessObjectToCache(Type businessObjectType, string cacheKey, BusinessObjectBase businessObject, CacheOptions options = null);

		/// <summary>
		/// Vyhledá v cache položku, která je business objektem (cachování readonly objektů).
		/// </summary>
		BusinessObjectBase GetBusinessObjectFromCache(Type businessObjectType, string cacheKey);

		/// <summary>
		/// Přidá do cache položku, která je DataRecordem (cachování non-readonly objektů).
		/// </summary>
		void AddDataRecordToCache(Type businessObjectType, string cacheKey, DataRecord dataRecord, CacheOptions options = null);

		/// <summary>
		/// Vyhledá v cache položku, která je DataRecordem (cachování non-readonly objektů).
		/// </summary>
		DataRecord GetDataRecordFromCache(Type businessObjectType, string cacheKey);

		/// <summary>
		/// Odstraní z cache položku, která je DataRecordem (cachování non-readonly objektů).
		/// </summary>
		void RemoveDataRecordFromCache(Type businessObjectType, string cacheKey);

		/// <summary>
		/// Přidá do cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
		/// </summary>
		void AddAllIDsToCache(Type businessObjectType, string cacheKey, int[] ids, CacheOptions options = null);

		/// <summary>
		/// Vyhledá v cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
		/// </summary>
		int[] GetAllIDsFromCache(Type businessObjectType, string cacheKey);

		/// <summary>
		/// Odstraní z cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
		/// </summary>
		void RemoveAllIDsFromCache(Type businessObjectType, string cacheKey);

		/// <summary>
		/// Přidá do cache položku, která je projekcí databázových resources pro jeden jazyk.
		/// </summary>
		void AddDbResourcesDataToCache(string cacheKey, object resources, CacheOptions cacheOptions);

		/// <summary>
		/// Odstraní z cache položku, která je projekcí databázových resources pro jeden jazyk.
		/// </summary>
		object GetDbResourcesDataFromCache(string cacheKey);

	}
}
