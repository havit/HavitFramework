using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Microsoft.Extensions.Logging;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader;

public static class DataLoaderTestHelper
{
	public static DbDataLoader CreateDataLoader(IDbContext dbContext = null, IEntityCacheManager entityCacheManager = null)
	{
		if (dbContext == null)
		{
			dbContext = new DataLoaderTestDbContext();
		}

		if (entityCacheManager == null)
		{
			entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext);
		}

		Mock<ILogger<DbDataLoader>> loggerMock = new Mock<ILogger<DbDataLoader>>(MockBehavior.Loose); // dovolíme použití loggeru bez setupu
		return new DbDataLoader(dbContext, new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), entityCacheManager, new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext), loggerMock.Object);
	}
}
