using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Havit.Business;
using Havit.Business.Caching;
using Havit.Business.Query;
using Havit.BusinessLayerTest;
using Havit.BusinessLayerTest.Resources;
using Havit.Data;
using Havit.Services.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.Tests
{
	[TestClass]
	public class BusinessObject_CachingTests
	{
		private const int CurrencyWellKnownID = 1;

		private IdentityMapScope identityMapScope;
		private ICacheService cacheService;

		[TestInitialize]
		public void TestInitialize()
		{
			//cacheService = new MemoryCacheService("Havit.BusinessTest");
			cacheService = new HttpRuntimeCacheService();
			cacheService.Clear();
			BusinessLayerContext.SetBusinessLayerCacheService(new DefaultBusinessLayerCacheService(cacheService));

			identityMapScope = new IdentityMapScope();
		}

		[TestCleanup]
		public void TestCleanup()		
		{
			identityMapScope.Dispose();
		}

		[TestMethod]
		public void BusinessObject_Load_SetsDataRecordToCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetDataRecordCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { CurrencyWellKnownID }));
			
			// Precondition
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Act
			Currency.GetObject(CurrencyWellKnownID).Load();

			// Assert
			Assert.IsTrue(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_GetAll_SetsAllRecordsCacheItemToCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetAllIDsCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null));

			// Precondition
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Act
			Currency.GetAll();

			// Assert
			Assert.IsTrue(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_RemovesDataRecordFromCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetDataRecordCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { CurrencyWellKnownID }));

			Currency currency = Currency.GetObject(CurrencyWellKnownID);			
			currency.Load();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Delete_RemovesDataRecordFromCache()
		{
			// Arrange
			Currency currency1 = Currency.CreateObject();
			currency1.Save();

			// hack - potřebujeme načíst objekt do identity mapy (jenže on už tam je). Tak existující identity mapu zahodíme a uděláme si novou.
			identityMapScope.Dispose();
			identityMapScope = new IdentityMapScope();
			// end of hack

			Currency currency2 = Currency.GetObject(currency1.ID);
			currency2.Load();

			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetDataRecordCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { currency2.ID }));

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency2.Delete();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_ExistingObject_DoesNotRemoveAllRecordsCacheItemFromCacheForNonSoftDeletableObjects()
		{
			// Poznámka: K vyhození z cache dojde pouze v případě označení objektu za smazaný (ev. naopak k označení smazaného objektu za nesmazaný).

			// Arrange
			CurrencyCollection currencies = Currency.GetAll();
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetAllIDsCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null));

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act			
			Currency currency = currencies.First();
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsTrue(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_NewObject_RemovesAllRecordsCacheItemFromCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetAllIDsCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null));

			// Precondition
			CurrencyCollection currencies = Currency.GetAll();
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			Currency currency = Currency.CreateObject();
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Clean up
			currency.Delete();
		}

		[TestMethod]
		public void BusinessObject_Delete_RemovesAllRecordsCacheItemFromCache()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetAllIDsCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null));
			CurrencyCollection currencies = Currency.GetAll();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Delete();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_ExistingObject_InvalidatesSaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.GetObject(CurrencyWellKnownID);
			currency.Load();

			string cacheKey = currency.GetSaveCacheDependencyKey();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_ExistingObject_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.GetObject(CurrencyWellKnownID);
			currency.Load();

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Save_NewObject_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();			

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Cleanup
			currency.Delete();
		}

		[TestMethod]
		public void BusinessObject_Delete_InvalidatesSaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = currency.GetSaveCacheDependencyKey();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Delete();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_Delete_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsTrue(cacheService.Contains(cacheKey));

			// Act
			currency.Delete();

			// Assert
			Assert.IsFalse(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void BusinessObject_GetObject_ReadOnly_SetsObjectToCache()
		{
			// Arrange			
			string cacheKey = "BL|Role|1";

			// Precondition
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Act
			Role.GetObject(1);

			// Assert
			Assert.IsTrue(cacheService.Contains(cacheKey));
		}

		[TestMethod]
		public void DbResources_GetString_SetsResourcesToCache()
		{
			// Arrange
			CultureInfo cultureInfo = CultureInfo.GetCultureInfo("cs-CZ");
			string cacheKey = (string)(typeof(DbResources).GetMethod("GetDbResourcesDataCacheKey", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(DbResources.Current, new object[] { cultureInfo }));

			// Precondition
			Assert.IsFalse(cacheService.Contains(cacheKey));

			// Act
			DbResources.Current.GetString("MainResourceClass", "MainResourceKey", cultureInfo);

			// Assert
			Assert.IsTrue(cacheService.Contains(cacheKey));
		}
	}
}
