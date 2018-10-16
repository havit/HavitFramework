using Havit.Data.EntityFrameworkCore.Patterns.Caching;
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
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);
			
			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			ICacheService cacheService = new NullCacheService();

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(language)).Returns(false);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheService, entityCacheSupportDecisionMock.Object, new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheService), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

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

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, new CacheAllEntitiesEntityCacheSupportDecision(), new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

			entityCacheManager.StoreEntity(language);

			// Assert
			cacheServiceMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_Store_DoesNotCallCacheServiceAddWhenShouldNotCache()
		{
			TestDbContext testDbContext = new TestDbContext();
			Language language = new Language { Id = 100 };
			testDbContext.Attach(language);

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(language)).Returns(false);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, entityCacheSupportDecisionMock.Object, new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

			entityCacheManager.StoreEntity(language);

			// Assert
			cacheServiceMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Never);
		}

		[TestMethod]
		public void EntityCacheManager_TryGet_CallsCacheServiceTryGetWhenCouldCache()
		{
			TestDbContext testDbContext = new TestDbContext();

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			object tryGetOutParameter;
			cacheServiceMock.Setup(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter)).Returns(false);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, new CacheAllEntitiesEntityCacheSupportDecision(), new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

			var result = entityCacheManager.TryGetEntity<Language>(555, out Language langauge);

			// Assert
			cacheServiceMock.Verify(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_TryGet_DoesNotCallCacheServiceTryGetWhenShouldNotCache()
		{
			TestDbContext testDbContext = new TestDbContext();

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			object tryGetOutParameter;
			cacheServiceMock.Setup(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter)).Returns(false);

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity<Language>()).Returns(false);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, entityCacheSupportDecisionMock.Object, new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

			var result = entityCacheManager.TryGetEntity<Language>(555, out Language langauge);

			// Assert
			cacheServiceMock.Verify(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter), Times.Never);
		}

		[TestMethod]
		public void EntityCacheManager_Scenarion_Store_And_TryGet()
		{
			ICacheService cacheService = new DictionaryCacheService();

			Func<IDbContext, EntityCacheManager> entityCacheManagerCreator = dbContext =>
			{
				Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
				dbContextFactoryMock.Setup(m => m.CreateService()).Returns(dbContext);
				dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

				Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
				entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

				return new EntityCacheManager(cacheService, new CacheAllEntitiesEntityCacheSupportDecision(), new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheService), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);
			};

			TestDbContext dbContext1 = new TestDbContext();
			var entityCacheManager1 = entityCacheManagerCreator(dbContext1);

			TestDbContext dbContext2 = new TestDbContext();
			var entityCacheManager2 = entityCacheManagerCreator(dbContext2);

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

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator();
			string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), language.Id);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, new CacheAllEntitiesEntityCacheSupportDecision(), entityCacheKeyGenerator, entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

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

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator();
			string cacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), language.Id);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, new CacheAllEntitiesEntityCacheSupportDecision(), entityCacheKeyGenerator, entityCacheOptionsGeneratorMock.Object, new EntityCacheDependencyManager(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

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

			Mock<IServiceFactory<IDbContext>> dbContextFactoryMock = new Mock<IServiceFactory<IDbContext>>(MockBehavior.Strict);
			dbContextFactoryMock.Setup(m => m.CreateService()).Returns(testDbContext);
			dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(true);

			EntityCacheDependencyManager entityCacheDependencyManager = new EntityCacheDependencyManager(cacheServiceMock.Object);
			string cacheKey = entityCacheDependencyManager.GetSaveCacheDependencyKey(typeof(Language), language.Id);

			Mock<IEntityCacheOptionsGenerator> entityCacheOptionsGeneratorMock = new Mock<IEntityCacheOptionsGenerator>(MockBehavior.Strict);
			entityCacheOptionsGeneratorMock.Setup(m => m.GetEntityCacheOptions<Language>(It.IsAny<Language>())).Returns((CacheOptions)null);

			EntityCacheManager entityCacheManager = new EntityCacheManager(cacheServiceMock.Object, new CacheAllEntitiesEntityCacheSupportDecision(), new EntityCacheKeyGenerator(), entityCacheOptionsGeneratorMock.Object, entityCacheDependencyManager, new DbEntityKeyAccessor(dbContextFactoryMock.Object), dbContextFactoryMock.Object);

			// Act
			entityCacheManager.InvalidateEntity(Patterns.UnitOfWorks.ChangeType.Update, language);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(cacheKey), Times.Once); // volá se ještě pro entitu a AllKeys, tak musíme kontrolovat jen klíč pro entitu
		}

		private class DictionaryCacheService : ICacheService
		{
			private Dictionary<string, object> values = new Dictionary<string, object>();

			public bool SupportsCacheDependencies => false;

			public void Add(string key, object value, CacheOptions options = null)
			{
				values[key] = value;
			}

			public void Clear()
			{
				values = new Dictionary<string, object>();
			}

			public bool Contains(string key)
			{
				return values.ContainsKey(key);
			}

			public void Remove(string key)
			{
				values.Remove(key);
			}

			public bool TryGet(string key, out object result)
			{
				return values.TryGetValue(key, out result);
			}
		}
	}
}
