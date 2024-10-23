using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <inheritdoc />
public class AnnotationsEntityCacheOptionsGeneratorStorageBuilder : IAnnotationsEntityCacheOptionsGeneratorStorageBuilder
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsEntityCacheOptionsGeneratorStorageBuilder(IDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public IAnnotationsEntityCacheOptionsGeneratorStorage Build()
	{
		return new AnnotationsEntityCacheOptionsGeneratorStorage
		{
			Value = _dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
				entityType => entityType.ClrType,
				entityType =>
				{
					var options = GetCacheOptions(entityType);
					options?.Freeze();
					return options;
				})
		};
	}

	/// <summary>
	/// Vrací cache options pro danou entitu.
	/// Neočekává se sdílená instance přes různé typy. CacheOptions jsou následně uzamčeny pro změnu.
	/// </summary>
	protected virtual CacheOptions GetCacheOptions(IReadOnlyEntityType entityType)
	{
		int? absoluteExpiration = (int?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.AbsoluteExpirationAnnotationName)?.Value;
		int? slidingExpiration = (int?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.SlidingExpirationAnnotationName)?.Value;
		Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority? priority = (Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.PriorityAnnotationName)?.Value;

		if ((absoluteExpiration != null) || (slidingExpiration != null) || (priority != null))
		{
			return new CacheOptions
			{
				AbsoluteExpiration = (absoluteExpiration == null) ? (TimeSpan?)null : TimeSpan.FromSeconds(absoluteExpiration.Value),
				SlidingExpiration = (slidingExpiration == null) ? (TimeSpan?)null : TimeSpan.FromSeconds(slidingExpiration.Value),
				Priority = GetPriority(priority)
			};
		}

		return null;
	}

	private Havit.Services.Caching.CacheItemPriority GetPriority(Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority? priority)
	{
		switch (priority)
		{
			case null:
			case Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority.Normal:
				return Havit.Services.Caching.CacheItemPriority.Normal;

			case Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority.Low:
				return Havit.Services.Caching.CacheItemPriority.Low;

			case Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority.High:
				return Havit.Services.Caching.CacheItemPriority.High;

			case Havit.Data.EntityFrameworkCore.Attributes.CacheItemPriority.NotRemovable:
				return Havit.Services.Caching.CacheItemPriority.NotRemovable;

			default:
				throw new InvalidOperationException(priority.ToString());
		}
	}

}
