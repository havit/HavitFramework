using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <inheritdoc />
public class AnnotationsEntityCacheSupportDecisionStorageBuilder : IAnnotationsEntityCacheSupportDecisionStorageBuilder
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsEntityCacheSupportDecisionStorageBuilder(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public IAnnotationsEntityCacheSupportDecisionStorage Build()
	{
		return new AnnotationsEntityCacheSupportDecisionStorage
		{
			ShouldCacheEntities = _dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
						entityType => entityType.ClrType,
						entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheAllKeysAnnotationName)?.Value)).GetValueOrDefault(false)),

			ShouldCacheAllKeys = _dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
						entityType => entityType.ClrType,
						entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false))
		};
	}
}
