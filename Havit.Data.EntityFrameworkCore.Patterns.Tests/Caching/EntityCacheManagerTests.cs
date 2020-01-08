using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Services;
using Havit.Services.Caching;
using Havit.Services.TestHelpers.Caching;
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
            CachingTestDbContext dbContext = new CachingTestDbContext();
            Role role = new Role { Id = 100 };
            dbContext.Attach(role);
			
			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(role)).Returns(false);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: dbContext,
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object
            );

            // Act
			entityCacheManager.StoreEntity(role);

			// Assert
			entityCacheSupportDecisionMock.Verify(m => m.ShouldCacheEntity(role), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_Store_CallsCacheServiceAddWhenShouldCache()
		{
			CachingTestDbContext dbContext = new CachingTestDbContext();
            Role role = new Role { Id = 100 };
            dbContext.Attach(role);

            Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: dbContext,
                cacheService: cacheServiceMock.Object);

			entityCacheManager.StoreEntity(role);

			// Assert
			cacheServiceMock.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_Store_DoesNotCallCacheServiceAddWhenShouldNotCache()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
            Role role = new Role { Id = 100 };
            dbContext.Attach(role);

            Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));

			Mock<IEntityCacheSupportDecision> entityCacheSupportDecisionMock = new Mock<IEntityCacheSupportDecision>(MockBehavior.Strict);
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntity(role)).Returns(false);

            EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: dbContext,
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object,
                cacheService: cacheServiceMock.Object);

            // Act
			entityCacheManager.StoreEntity(role);

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
            var result = entityCacheManager.TryGetEntity<Role>(555, out Role role);

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
			entityCacheSupportDecisionMock.Setup(m => m.ShouldCacheEntityType(typeof(Role))).Returns(false);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                cacheService: cacheServiceMock.Object, 
                entityCacheSupportDecision: entityCacheSupportDecisionMock.Object);

            // Act
            var result = entityCacheManager.TryGetEntity<Role>(555, out Role langauge);

			// Assert
			cacheServiceMock.Verify(m => m.TryGet(It.IsAny<string>(), out tryGetOutParameter), Times.Never);
		}
        
		[TestMethod]
		public void EntityCacheManager_Scenarion_Store_And_TryGet()
		{
            // Arrange
			ICacheService cacheService = new DictionaryCacheService();

			CachingTestDbContext dbContext1 = new CachingTestDbContext();
			var entityCacheManager1 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext1, cacheService: cacheService);

			CachingTestDbContext dbContext2 = new CachingTestDbContext();
			var entityCacheManager2 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext2, cacheService: cacheService);

            Role role = new Role { Id = 100, Name = "Reader" };
			dbContext1.Attach(role);

			// Act
			entityCacheManager1.StoreEntity(role);
			var success = entityCacheManager2.TryGetEntity<Role>(role.Id, out Role roleResult);

			// Assert
			Assert.IsTrue(success);
			Assert.IsNotNull(roleResult);
			Assert.AreNotSame(role, roleResult);
			Assert.AreEqual(roleResult.Name, roleResult.Name);
			Assert.AreEqual(dbContext1.Entry(role).CurrentValues.GetValue<string>(nameof(Role.Name)), dbContext2.Entry(roleResult).CurrentValues.GetValue<string>(nameof(Role.Name)));
			Assert.AreEqual(dbContext1.Entry(role).OriginalValues.GetValue<string>(nameof(Role.Name)), dbContext2.Entry(roleResult).OriginalValues.GetValue<string>(nameof(Role.Name)));
			Assert.AreEqual(Microsoft.EntityFrameworkCore.EntityState.Unchanged, dbContext2.Entry(roleResult).State);
		}

        [TestMethod]
        public void EntityCacheManager_Scenarion_StoreAllKeys_And_TryGetAllKeys()
        {
            // Arrange
            ICacheService cacheService = new DictionaryCacheService();

            var entityCacheManager1 = CachingTestHelper.CreateEntityCacheManager(cacheService: cacheService);
            var entityCacheManager2 = CachingTestHelper.CreateEntityCacheManager(cacheService: cacheService);

            object allKeys = new object(); // just a marker object
            // Act
            entityCacheManager1.StoreAllKeys<Role>(allKeys);
            bool success = entityCacheManager2.TryGetAllKeys<Role>(out object allKeysResult);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(allKeysResult);
            Assert.AreSame(allKeys, allKeysResult);
        }

        [TestMethod]
        public void EntityCacheManager_Scenarion_OneToMany_StoreCollection_And_TryGetCollection()
        {
            // Arrange
            ICacheService cacheService = new DictionaryCacheService();

			CachingTestDbContext dbContext1 = new CachingTestDbContext();

            Master master = new Master { Id = 1 };
            Child child1 = new Child { Id = 100, ParentId = 1, Parent = master };
            Child child2 = new Child { Id = 101, ParentId = 1, Parent = master, Deleted = DateTime.Now };
            master.ChildrenWithDeleted.Add(child1);
            master.ChildrenWithDeleted.Add(child2);
            dbContext1.Attach(master);

            var entityCacheManager1 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext1, cacheService: cacheService);

			CachingTestDbContext dbContext2 = new CachingTestDbContext();
            Master masterResult = new Master { Id = 1 };
            dbContext2.Attach(masterResult);

            var entityCacheManager2 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext2, cacheService: cacheService);

            // Act
            entityCacheManager1.StoreCollection<Master, Child>(master, nameof(Master.ChildrenWithDeleted));
            entityCacheManager1.StoreEntity(child1);
            entityCacheManager1.StoreEntity(child2);
            bool success = entityCacheManager2.TryGetCollection<Master, Child>(masterResult, nameof(Master.ChildrenWithDeleted));

            // Assert
            Assert.IsTrue(success, "Načtění kolekce z cache nebylo úspěšné.");
            Assert.AreEqual(master.ChildrenWithDeleted.Count, masterResult.ChildrenWithDeleted.Count);
            Assert.IsTrue(masterResult.ChildrenWithDeleted.Any(child => child.Id == child1.Id));
            Assert.IsTrue(masterResult.ChildrenWithDeleted.Any(child => child.Id == child2.Id));
            Assert.AreEqual(4, master.ChildrenWithDeleted.Union(masterResult.ChildrenWithDeleted).Distinct().Count()); // nejsou sdílené žádné instance (tj. master.Children[0] != master.Children[1] != masterResult.Children[0] != masterResult.Children[1]
        }

        [TestMethod]
        public void EntityCacheManager_Scenarion_ManyToMany_StoreCollection_And_TryGetCollection()
        {
            // Arrange
            ICacheService cacheService = new DictionaryCacheService();

			CachingTestDbContext dbContext1 = new CachingTestDbContext();
            
            LoginAccount loginAccount = new LoginAccount { Id = 1 };
            Membership membership = new Membership { LoginAccountId = 1, RoleId = 1234 };
            loginAccount.Memberships = new List<Membership> { membership };

            var entityCacheManager1 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext1, cacheService: cacheService);

			CachingTestDbContext dbContext2 = new CachingTestDbContext();
            LoginAccount loginAccountResult = new LoginAccount { Id = 1 };
            dbContext2.Attach(loginAccountResult);

            var entityCacheManager2 = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext2, cacheService: cacheService);

            // Act
            entityCacheManager1.StoreCollection<LoginAccount, Membership>(loginAccount, nameof(LoginAccount.Memberships));
            bool success = entityCacheManager2.TryGetCollection<LoginAccount, Membership>(loginAccountResult, nameof(LoginAccount.Memberships));

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(1, loginAccountResult.Memberships.Count);
            Assert.AreEqual(membership.RoleId, loginAccountResult.Memberships[0].RoleId);
            Assert.AreNotSame(loginAccount.Memberships[0], loginAccountResult.Memberships[0]);
        }
        
        [TestMethod]
		public void EntityCacheManager_InvalidateEntity_RemovesEntityAndAllKeysOnUpdate()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
            LoginAccount loginAccount = new LoginAccount { Id = 1 };
            dbContext.Attach(loginAccount);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator(dbContext.CreateDbContextFactory());
			string entityCacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(LoginAccount), loginAccount.Id);
			string allKeysCacheKey = entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(LoginAccount));

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: dbContext,
                cacheService: cacheServiceMock.Object,
                entityCacheKeyGenerator: entityCacheKeyGenerator);

			Changes changes = new Changes
			{
				Inserts = new object[0],
				Updates = new object[] { loginAccount },
				Deletes = new object[0]
			};

			// Act
			entityCacheManager.Invalidate(changes);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(entityCacheKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(allKeysCacheKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(It.IsAny<string>()), Times.Exactly(2)); // a nic víc
		}
        
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_DoesNotRemoveEntityButRemovesAllKeysOnInsert()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
            LoginAccount loginAccount = new LoginAccount { Id = 1 };
            dbContext.Attach(loginAccount);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator(dbContext.CreateDbContextFactory());
			string entityCacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(LoginAccount), loginAccount.Id);
			string allKeysCacheKey = entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(LoginAccount));

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
                dbContext: dbContext,
                cacheService: cacheServiceMock.Object,
                entityCacheKeyGenerator: entityCacheKeyGenerator);

			Changes changes = new Changes
			{
				Inserts = new object[] { loginAccount },
				Updates = new object[0],
				Deletes = new object[0]
			};

			// Act
			entityCacheManager.Invalidate(changes);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(entityCacheKey), Times.Never);
			cacheServiceMock.Verify(m => m.Remove(allKeysCacheKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(It.IsAny<string>()), Times.Once); // a nic víc

		}

		// Bug #44100: Cachování - EntityCacheManager se s invalidací snaží uložit do cache i právě mazanou entitu
		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_DoesNotStoreDeletedEntity()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			LoginAccount loginAccount = new LoginAccount { Id = 1 };

			// deleted entity, simulation of UnitOfWork.AddForDelete() + Commit()
			dbContext.Add(loginAccount);
			dbContext.SaveChanges();
			dbContext.Remove(loginAccount);
			dbContext.SaveChanges();

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(false);

			var entityCacheKeyGenerator = new EntityCacheKeyGenerator(dbContext.CreateDbContextFactory());
			string entityCacheKey = entityCacheKeyGenerator.GetEntityCacheKey(typeof(LoginAccount), loginAccount.Id);
			string allKeysCacheKey = entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(LoginAccount));

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(
				dbContext: dbContext,
				cacheService: cacheServiceMock.Object,
				entityCacheKeyGenerator: entityCacheKeyGenerator);

			Changes changes = new Changes
			{
				Inserts = new object[0],
				Updates = new object[0],
				Deletes = new object[] { loginAccount },
			};

			// Act
			entityCacheManager.Invalidate(changes);

			// Assert
			cacheServiceMock.Verify(m => m.Add(entityCacheKey, It.IsAny<object>(), It.IsAny<CacheOptions>()), Times.Never);
			cacheServiceMock.Verify(m => m.Remove(entityCacheKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(allKeysCacheKey), Times.Once);
		}

		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_SupportsManyToMany()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			Membership membership = new Membership { LoginAccountId = 100, RoleId = 999 };
            dbContext.Attach(membership);

            EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext);

			Changes changes = new Changes
			{
				Inserts = new object[0],
				Updates = new object[0],
				Deletes = new object[] { membership }
			};
			// Act
			entityCacheManager.Invalidate(changes);

			// Assert
			// no exception was thrown
		}

		[TestMethod]
		public void EntityCacheManager_InvalidateEntity_SupportsNotRequiredForeignKeyWithNullValue()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			Child child = new Child { ParentId = null };

			dbContext.Attach(child);

			EntityCacheManager entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext: dbContext);

			Changes changes = new Changes
			{
				Inserts = new object[0],
				Updates = new object[] { child },
				Deletes = new object[0]
			};
			// Act
			entityCacheManager.Invalidate(changes);

			// Assert
			// no exception was thrown
		}
	}
}
