using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching
{
	[TestClass]
	public class EntityCacheManagerTests
	{
		[TestMethod]
		public void EntityCacheManager_Store_CallsShouldCacheEntity()
		{
            // Arrange
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);
			
			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(language)).Returns(false);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object
            );

            // Act
			entityCacheManager.StoreEntity(language);

			// Assert
			entityCacheSupportDecisionMock.Verify(m => m.ShouldCacheEntity(language), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_Store_CallsCacheServiceAddWhenShouldCache()
		{
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);
			
			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                cacheService: cacheServiceMock.Object);

			entityCacheManager.StoreEntity(language);

			// Assert
			cacheServiceMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_Store_DoesNotCallCacheServiceAddWhenShouldNotCache()
		{
            // Arrange
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(language)).Returns(false);

            EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object,
                cacheService: cacheServiceMock.Object);

            // Act
			entityCacheManager.StoreEntity(language);

			// Assert
			cacheServiceMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Never);
		}
        
		[TestMethod]
		public void EntityCacheManager_TryGet_CallsCacheServiceTryGetWhenCouldCache()
		{
            // Arrange
			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			object tryGetOutParameter;
			cacheServiceMock.Setup(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter)).Returns(false);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(cacheService: cacheServiceMock.Object);

            // Act
            var result = entityCacheManager.TryGetEntity<Language>(555, out Language langauge);

			// Assert
			cacheServiceMock.Verify(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter), Times.Once);
		}
        
		[TestMethod]
		public void EntityCacheManager_TryGet_DoesNotCallCacheServiceTryGetWhenShouldNotCache()
		{
            // Arrange
			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			object tryGetOutParameter;
			cacheServiceMock.Setup(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter)).Returns(false);

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity<Language>()).Returns(false);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                cacheService: cacheServiceMock.Object, 
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object);

            // Act
            var result = entityCacheManager.TryGetEntity<Language>(555, out Language langauge);

			// Assert
			cacheServiceMock.Verify(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter), Times.Never);
		}
        
		[TestMethod]
		public void EntityCacheManager_Scenarion_Store_And_TryGet()
		{
            // Arrange
			ICacheService cacheService = new DictionaryCacheService();

			TestDbContext dbContext1 = new TestDbContext();
			var entityCacheManager1 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext1, cacheService: cacheService);

			TestDbContext dbContext2 = new TestDbContext();
			var entityCacheManager2 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext2, cacheService: cacheService);

            Language language = new Language { Id = 100, Culture = "cs-cz" };
			dbContext1.Attach(language);

			// Act
			entityCacheManager1.StoreEntity(language);
			var success = entityCacheManager2.TryGetEntity<Language>(language.Id, out Language languageResult);

			// Assert
			Assert.IsTrue(success);
			Assert.IsNotNull(languageResult);
			Assert.AreNotSame(language, languageResult);
			Assert.AreEqual(language.Culture, languageResult.Culture);
			Assert.AreEqual(dbContext1.Entry(language).CurrentValues.GetValue<string>(nameof(Language.Culture)), dbContext2.Entry(languageResult).CurrentValues.GetValue<string>(nameof(Language.Culture)));
			Assert.AreEqual(dbContext1.Entry(language).OriginalValues.GetValue<string>(nameof(Language.Culture)), dbContext2.Entry(languageResult).OriginalValues.GetValue<string>(nameof(Language.Culture)));
			Assert.AreEqual(Microsoft.EntityFrameworkCore.EntityState.Unchanged, dbContext2.Entry(languageResult).State);
		}
        
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_RemovesEntityOnUpdate()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator();
			string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), language.Id);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                cacheService: cacheServiceMock.Object,
                entityCacheKeyGenerator: entityCacheKeyGenerator);

			// Act
			entityCacheManager.InvalidateEntity(Patterns.UnitOfWorks.ChangeType.Update, language);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(cacheKey), Times.Once); // volá se ještě pro AllKeys, tak musíme kontrolovat jen klíč pro entitu
		}
        
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_DoesNotRemoveEntityOnInsert()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator();
			string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), language.Id);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                cacheService: cacheServiceMock.Object,
                entityCacheKeyGenerator: entityCacheKeyGenerator);

			// Act
			entityCacheManager.InvalidateEntity(Patterns.UnitOfWorks.ChangeType.Insert, language);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(cacheKey), Times.Never); // volá se ještě pro AllKeys, tak musíme kontrolovat jen klíč pro entitu
		}
        
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_RemovesDependencies()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(true);

			EntityCacheDependencyManager entityCacheDependencyManager = new EntityCacheDependencyManager(cacheServiceMock.Object);
			string cacheKey = entityCacheDependencyManager.GetSaveCacheDependencyKey(typeof(Language), language.Id);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: testDbContext,
                cacheService: cacheServiceMock.Object,
                entityCacheDependencyManager: entityCacheDependencyManager);

			// Act
			entityCacheManager.InvalidateEntity(Patterns.UnitOfWorks.ChangeType.Update, language);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(cacheKey), Times.Once); // volá se ještě pro entitu a AllKeys, tak musíme kontrolovat jen klíč pro entitu
		}
       
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_SupportsManyToMany()
		{
			// Arrange
			TestDbContext testDbContext = new TestDbContext();
			ManyToMany manyToMany = new ManyToMany { LanguageId = 100, ItemWithDeletedId = 999 };
			testDbContext.Attach(manyToMany);

            EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext: testDbContext);

			// Act
			entityCacheManager.InvalidateEntity(Patterns.UnitOfWorks.ChangeType.Delete, manyToMany);

			// Assert
			// no exception was thrown
		}       
	}
}
