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

		INavigationTargetStorage navigationTargetStorage = new NavigationTargetStorageBuilder(dbContext).Build();
		IAnnotationsEntityCacheOptionsGeneratorStorage annotationsEntityCacheOptionsGeneratorStorage = new AnnotationsEntityCacheOptionsGeneratorStorageBuilder(dbContext).Build();
		if (entityCacheOptionsGenerator == null)
		{
			entityCacheOptionsGenerator = new AnnotationsEntityCacheOptionsGenerator(annotationsEntityCacheOptionsGeneratorStorage, new NavigationTargetService(navigationTargetStorage));
		}

		if (entityCacheKeyGenerator == null)
		{
			IEntityCacheKeyPrefixStorage entityCacheKeyPrefixStorage = new EntityCacheKeyPrefixStorageBuilder(dbContext).Build();
			entityCacheKeyGenerator = new EntityCacheKeyGenerator(new EntityCacheKeyPrefixService(entityCacheKeyPrefixStorage));
		}

		if (cacheService == null)
		{
			cacheService = new NullCacheService();
		}

		IPropertyLambdaExpressionManager propertyLambdaExpressionManager = new PropertyLambdaExpressionManager(new PropertyLambdaExpressionStore(), new PropertyLambdaExpressionBuilder());
		IReferencingNavigationsStorage referencingNavigationsStorage = new ReferencingNavigationsStorageBuilder(dbContext).Build();
		IReferencingNavigationsService referencingCollectionStore = new ReferencingNavigationsService(referencingNavigationsStorage);

		IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage = new DbEntityKeyAccessorStorageBuilder(dbContext).Build();

		return new EntityCacheManager(
			cacheService,
			entityCacheSupportDecision,
			entityCacheKeyGenerator,
			entityCacheOptionsGenerator,
			new DbEntityKeyAccessor(dbEntityKeyAccessorStorage),
			propertyLambdaExpressionManager,
			dbContext,
			referencingCollectionStore,
			new NavigationTargetService(navigationTargetStorage));
	}
}
