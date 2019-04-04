using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
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
                    ICacheService cacheService = null)
        {
            if (dbContext == null)
            {
                dbContext = new DataLoaderTestDbContext();
            }

            if (entityCacheSupportDecision == null)
            {
                entityCacheSupportDecision = new CacheAllEntitiesEntityCacheSupportDecision();
            }

            if (entityCacheOptionsGenerator == null)
            {
                entityCacheOptionsGenerator = new AnnotationsEntityCacheOptionsGenerator(dbContext.CreateDbContextFactory(), new CollectionTargetTypeStore(dbContext.CreateDbContextFactory()));
            }

            if (entityCacheKeyGenerator == null)
            {
                entityCacheKeyGenerator = new EntityCacheKeyGenerator(dbContext.CreateDbContextFactory());
            }

            if (cacheService == null)
            {
                cacheService = new NullCacheService();
            }

            IPropertyLambdaExpressionManager propertyLambdaExpressionManager = new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder());
            IReferencingCollectionsStore referencingCollectionStore = new ReferencingCollectionsStore(dbContext.CreateDbContextFactory());

            return new EntityCacheManager(
                cacheService,
                entityCacheSupportDecision,
                entityCacheKeyGenerator,
                entityCacheOptionsGenerator,
                new DbEntityKeyAccessor(dbContext.CreateDbContextFactory()),
                propertyLambdaExpressionManager,
                dbContext,
                referencingCollectionStore);
        }
	}
}
