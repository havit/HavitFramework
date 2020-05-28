using Havit.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.Tests.Caching
{
	[TestClass]
	public class MemoryCacheServiceTests
	{
		/// <summary>
		/// Čištění cache je implementováno reflexí, proto unittestem ověřujeme funkčnost.
		/// </summary>
		[TestMethod]
		public void MemoryCacheService_Clear()
		{
			string cacheKey = "CacheItem";

			MemoryCache memoryCache = new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
			MemoryCacheService memoryCacheService = new MemoryCacheService(memoryCache);
			memoryCacheService.Add(cacheKey, new object(), new CacheOptions { Priority = Services.Caching.CacheItemPriority.NotRemovable });

			// Precondition
			Assert.IsTrue(memoryCacheService.TryGet(cacheKey, out var _));

			// Act
			memoryCacheService.Clear();

			// Assert
			Assert.IsFalse(memoryCacheService.TryGet(cacheKey, out var _));
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void MemoryCacheService_Clear_ThrowsExceptionWhenClearNotPossible()
		{
			Mock<IMemoryCache> memoryCacheMock = new Mock<IMemoryCache>(MockBehavior.Strict); // nemá "EntriesCollection"!
			MemoryCacheService memoryCacheService = new MemoryCacheService(memoryCacheMock.Object);

			// Act
			memoryCacheService.Clear();

			// Assert by method attribute
		}
	}
}
