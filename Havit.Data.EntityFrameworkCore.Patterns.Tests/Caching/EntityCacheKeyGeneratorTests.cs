using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;

[TestClass]
public class EntityCacheKeyGeneratorTests
{
	[TestMethod]
	public void EntityCacheKeyGenerator_GetEntityCacheKey()
	{
		// Arrange
		IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage = new EntityCacheKeyPrefixStorageBuilder(new TestDbContext()).Build();
		EntityCacheKeyGenerator entityCacheKeyGenerator = new EntityCacheKeyGenerator(new EntityCacheKeyPrefixService(entityCacheKeyPrefixStorage));

		// Act + Assert
		Assert.AreEqual("EF|Language|5", entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), 5));
	}

	[TestMethod]
	public void EntityCacheKeyGenerator_GetAllKeysCacheKey()
	{
		// Arrange
		IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage = new EntityCacheKeyPrefixStorageBuilder(new TestDbContext()).Build();
		EntityCacheKeyGenerator entityCacheKeyGenerator = new EntityCacheKeyGenerator(new EntityCacheKeyPrefixService(entityCacheKeyPrefixStorage));

		// Act + Assert
		Assert.AreEqual("EF|Language|AllKeys", entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(Language)));
	}

}
