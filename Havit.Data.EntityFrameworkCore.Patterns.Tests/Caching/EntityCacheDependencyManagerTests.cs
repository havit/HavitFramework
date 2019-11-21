using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
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
	public class EntityCacheDependencyManagerTests
	{
		[TestMethod]
		public void EntityCacheDpendencyManager_InvalidateEntity_RemovesDependencies()
		{
			// Arrange
			CachingTestDbContext dbContext = new CachingTestDbContext();
			LoginAccount loginAccount = new LoginAccount { Id = 1 };
			dbContext.Attach(loginAccount);

			Mock<ICacheService> cacheServiceMock = new Mock<ICacheService>(MockBehavior.Strict);
			cacheServiceMock.Setup(m => m.Contains(It.IsAny<string>())).Returns(false);
			cacheServiceMock.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CacheOptions>()));
			cacheServiceMock.Setup(m => m.Remove(It.IsAny<string>()));
			cacheServiceMock.SetupGet(m => m.SupportsCacheDependencies).Returns(true);

			EntityCacheDependencyKeyGenerator entityCacheDependencyManager = new EntityCacheDependencyKeyGenerator(cacheServiceMock.Object);
			string saveCacheDependencyKey = entityCacheDependencyManager.GetSaveCacheDependencyKey(typeof(LoginAccount), loginAccount.Id);
			string anySaveCacheDependencyKey = entityCacheDependencyManager.GetAnySaveCacheDependencyKey(typeof(LoginAccount));

			EntityCacheDependencyManager entityCacheManager = new EntityCacheDependencyManager(cacheServiceMock.Object, new EntityCacheDependencyKeyGenerator(cacheServiceMock.Object), new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()));

			Changes changes = new Changes
			{
				Inserts = new object[0],
				Updates = new object[] { loginAccount },
				Deletes = new object[0],
			};

			// Act
			entityCacheManager.InvalidateDependencies(changes);

			// Assert
			cacheServiceMock.Verify(m => m.Remove(saveCacheDependencyKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(anySaveCacheDependencyKey), Times.Once);
			cacheServiceMock.Verify(m => m.Remove(It.IsAny<string>()), Times.Exactly(2)); // a nic víc
		}

	}
}
