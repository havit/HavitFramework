using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching
{
	[TestClass]
	public class EntityCacheKeyGeneratorTests
	{
		[TestMethod]
		public void EntityCacheKeyGenerator_GetEntityCacheKey()
		{
			// Arrange
			EntityCacheKeyGenerator entityCacheKeyGenerator = new EntityCacheKeyGenerator(new TestDbContext().CreateDbContextFactory());

			// Act + Assert
			Assert.AreEqual("EF|Language|5", entityCacheKeyGenerator.GetEntityCacheKey(typeof(Language), 5));
		}

		[TestMethod]
		public void EntityCacheKeyGenerator_GetAllKeysCacheKey()
		{
			// Arrange
			EntityCacheKeyGenerator entityCacheKeyGenerator = new EntityCacheKeyGenerator(new TestDbContext().CreateDbContextFactory());

			// Act + Assert
			Assert.AreEqual("EF|Language|AllKeys", entityCacheKeyGenerator.GetAllKeysCacheKey(typeof(Language)));
		}

	}
}
