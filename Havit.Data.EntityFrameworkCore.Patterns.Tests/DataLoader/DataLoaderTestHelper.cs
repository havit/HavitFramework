using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader
{
    public static class DataLoaderTestHelper
    {
        public static DbDataLoader CreateDataLoader(IDbContext dbContext = null, IEntityCacheManager entityCacheManager = null)
        {
            if (dbContext == null)
            {
                dbContext = new TestDbContext();
            }

            if (entityCacheManager == null)
            {
                entityCacheManager = CachingTestHelper.CreateEntityCacheManager(dbContext);
            }

            Mock<IDbContextFactory> dbContextFactoryMock = new Mock<IDbContextFactory>(MockBehavior.Strict);
            dbContextFactoryMock.Setup(m => m.CreateService()).Returns(dbContext);
            dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

            return new DbDataLoader(dbContext, new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution(), new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder()), entityCacheManager, new DbEntityKeyAccessor(dbContextFactoryMock.Object));
        }
    }
}
