using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching;

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
			dbContext = new CachingTestDbContext();
		}

		if (entityCacheSupportDecision == null)
		{
			entityCacheSupportDecision = new CacheAllEntitiesEntityCacheSupportDecision();
		}

		if (entityCacheOptionsGenerator == null)
		{
			entityCacheOptionsGenerator = new AnnotationsEntityCacheOptionsGenerator(new AnnotationsEntityCacheOptionsGeneratorStorage(), dbContext, new NavigationTargetTypeService(new NavigationTargetTypeStorage(), dbContext));
		}

		if (entityCacheKeyGenerator == null)
		{
			entityCacheKeyGenerator = new EntityCacheKeyGenerator(new EntityCacheKeyGeneratorStorage(), dbContext);
		}

		if (cacheService == null)
		{
			cacheService = new NullCacheService();
		}

		IPropertyLambdaExpressionManager propertyLambdaExpressionManager = new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder());
		IReferencingNavigationsService referencingCollectionStore = new ReferencingNavigationsService(new ReferencingNavigationsStorage(), dbContext);

		return new EntityCacheManager(
			cacheService,
			entityCacheSupportDecision,
			entityCacheKeyGenerator,
			entityCacheOptionsGenerator,
			new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), dbContext),
			propertyLambdaExpressionManager,
			dbContext,
			referencingCollectionStore);
	}
}
