﻿using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Services.Caching;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching
{
    public static class CachingTestHelper
    {
        public static EntityCacheManager CreateEntityCacheManager(
                    IDbContext dbContext = null,
                    IEntityCacheSupportDecision entityCacheSupportDecision = null,
                    IEntityCacheOptionsGenerator entityCacheOptionsGenerator = null,
                    IEntityCacheKeyGenerator entityCacheKeyGenerator = null,
                    ICacheService cacheService = null,
                    IEntityCacheDependencyManager entityCacheDependencyManager = null)
        {
            if (dbContext == null)
            {
                dbContext = new TestDbContext();
            }

            Mock<IDbContextFactory> dbContextFactoryMock = new Mock<IDbContextFactory>(MockBehavior.Strict);
            dbContextFactoryMock.Setup(m => m.CreateService()).Returns(dbContext);
            dbContextFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDbContext>()));

            if (entityCacheSupportDecision == null)
            {
                entityCacheSupportDecision = new CacheAllEntitiesEntityCacheSupportDecision();
            }

            if (entityCacheOptionsGenerator == null)
            {
                entityCacheOptionsGenerator = new AnnotationsEntityCacheOptionsGenerator(dbContextFactoryMock.Object);
            }

            if (entityCacheKeyGenerator == null)
            {
                entityCacheKeyGenerator = new EntityCacheKeyGenerator();
            }

            if (cacheService == null)
            {
                cacheService = new NullCacheService();
            }

            if (entityCacheDependencyManager == null)
            {
                entityCacheDependencyManager = new EntityCacheDependencyManager(cacheService);
            }

            IPropertyLambdaExpressionManager propertyLambdaExpressionManager = new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder());
            IReferencingCollectionsStore referencingCollectionStore = new ReferencingCollectionsStore(dbContextFactoryMock.Object);

            return new EntityCacheManager(
                cacheService,
                entityCacheSupportDecision,
                entityCacheKeyGenerator,
                entityCacheOptionsGenerator,
                entityCacheDependencyManager,
                new DbEntityKeyAccessor(dbContextFactoryMock.Object),
                propertyLambdaExpressionManager,
                dbContext,
                referencingCollectionStore);
        }
    }
}
