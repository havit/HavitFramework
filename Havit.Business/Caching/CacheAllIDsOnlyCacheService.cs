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
	/// Služba zajišťující cachování pouze GetAllIDs business objektu. Ostatní nejsou cachovány.
	/// Cache dependency není podporováno.
	/// </summary>
	/// <seealso cref="Havit.Business.Caching.IBusinessLayerCacheService" />
	public class CacheAllIDsOnlyCacheService : IBusinessLayerCacheService
	{
		private readonly ICacheService cacheService;

		/// <summary>
		/// Indikuje, zda podkladová cache podporuje cache dependencies.
		/// Tato třída vrací vždy false.
		/// </summary>
		public bool SupportsCacheDependencies
		{
			get { return false; }
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="cacheService">Instance cacheservice, která zprostředkovává přístup do cache.</param>
		public CacheAllIDsOnlyCacheService(ICacheService cacheService)
		{
			this.cacheService = cacheService;
		}

		/// <summary>
		/// Zajistí v cache přítomnost klíče, který je cache dependecy masterem pro závislé položky.
		/// Tato třída vždy vyhazuje výjimku NotSupportedException.
		/// </summary>
		public void EnsureCacheDependencyKey(Type businessObjectType, string cacheKey)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Invaliduje v cache klíče, který je cache dependecy masterem.
		/// Tato třída vždy vyhazuje výjimku NotSupportedException.
		/// </summary>
		public void InvalidateCacheDependencies(Type businessObjectType, string cacheKey)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Přidá do cache položku, která je business objektem (cachování readonly objektů).
		/// Tato třída nic neudělá.
		/// </summary>
		public void AddBusinessObjectToCache(Type businessObjectType, string cacheKey, BusinessObjectBase businessObject, CacheOptions options = null)
		{
			// NOOP
		}

		/// <summary>
		/// Vyhledá v cache položku, která je business objektem (cachování readonly objektů).
		/// Tato třída vždy vrací null.
		/// </summary>
		public BusinessObjectBase GetBusinessObjectFromCache(Type businessObjectType, string cacheKey)
		{
			return null;
		}

		/// <summary>
		/// Přidá do cache položku, která je DataRecordem (cachování non-readonly objektů).
		/// Tato třída nic neudělá.
		/// </summary>
		public void AddDataRecordToCache(Type businessObjectType, string cacheKey, DataRecord dataRecord, CacheOptions options)
		{
			// NOOP
		}

		/// <summary>
		/// Vyhledá v cache položku, která je business objektem (cachování readonly objektů).
		/// Tato třída vždy vrací null.
		/// </summary>
		public DataRecord GetDataRecordFromCache(Type businessObjectType, string cacheKey)
		{
			return null;
		}

		/// <summary>
		/// Odstraní z cache položku, která je DataRecordem (cachování non-readonly objektů).
		/// Tato třída nic neudělá.
		/// </summary>
		public void RemoveDataRecordFromCache(Type businessObjectType, string cacheKey)
		{
			// NOOP
		}

		/// <summary>
		/// Přidá do cache položku, která je kolekcí ID všech objektů (cachování kolekce GetAll).
		/// </summary>
		public void AddAllIDsToCache(Type businessObjectType, string cacheKey, int[] ids, CacheOptions options)
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
	}
}
