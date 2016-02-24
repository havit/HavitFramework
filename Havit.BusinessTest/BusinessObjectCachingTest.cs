using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Havit.Business;
using Havit.Business.Query;
using Havit.BusinessLayerTest;
using Havit.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.BusinessTest
{
	[TestClass]
	public class BusinessObjectCachingTest
	{
		private const int CurrencyWellKnownID = 1;

		private IdentityMapScope identityMapScope;

		[TestInitialize]
		public void TestInitialize()
		{
			ClearCache();
			identityMapScope = new IdentityMapScope();
		}

		[TestCleanup]
		public void TestCleanup()		
		{
			identityMapScope.Dispose();
		}

		[TestMethod]
		public void Load_SetsDataRecordToCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetDataRecordCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { CurrencyWellKnownID }));
			
			// Precondition
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);

			// Act
			Currency.GetObject(CurrencyWellKnownID).Load();

			// Assert
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void GetAll_SetsAllRecordsCacheItemToCache()
		{
			// Arrange
			string cacheKey = "Havit.BusinessLayerTest.Currency.GetAll";

			// Precondition
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);

			// Act
			Currency.GetAll();

			// Assert
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Save_RemovesDataRecordFromCache()
		{
			// Arrange
			string cacheKey = (string)(typeof(CurrencyBase).GetMethod("GetDataRecordCacheKey", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { CurrencyWellKnownID }));

			Currency currency = Currency.GetObject(CurrencyWellKnownID);			
			currency.Load();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Delete_RemovesDataRecordFromCache()
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
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency2.Delete();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Save_RemovesAllRecordsCacheItemFromCache()
		{
			// Arrange
			string cacheKey = "Havit.BusinessLayerTest.Currency.GetAll";

			// Precondition
			CurrencyCollection currencies = Currency.GetAll();
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			Currency currency = currencies.First();
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Delete_RemovesAllRecordsCacheItemFromCache()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = "Havit.BusinessLayerTest.Currency.GetAll";
			CurrencyCollection currencies = Currency.GetAll();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Delete();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Save_ExistingObject_InvalidatesSaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.GetObject(CurrencyWellKnownID);
			currency.Load();

			string cacheKey = currency.GetSaveCacheDependencyKey();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Save_ExistingObject_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.GetObject(CurrencyWellKnownID);
			currency.Load();

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Save_NewObject_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();			

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Nazev = Guid.NewGuid().ToString();
			currency.Save();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);

			// Cleanup
			currency.Delete();
		}

		[TestMethod]
		public void Delete_InvalidatesSaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = currency.GetSaveCacheDependencyKey();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Delete();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		[TestMethod]
		public void Delete_InvalidatesAnySaveCacheDependencyKey()
		{
			// Arrange
			Currency currency = Currency.CreateObject();
			currency.Save();

			string cacheKey = Currency.GetAnySaveCacheDependencyKey();

			// Precondition
			Assert.IsNotNull(HttpRuntime.Cache[cacheKey]);

			// Act
			currency.Delete();

			// Assert
			Assert.IsNull(HttpRuntime.Cache[cacheKey]);
		}

		private void ClearCache()
		{
			foreach (DictionaryEntry dictionaryEntry in HttpRuntime.Cache)
			{
				HttpRuntime.Cache.Remove((string)dictionaryEntry.Key);
			}
		}
	}
}
