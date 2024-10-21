using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Strategie definující, zda může být entita cachována. Řídí se anotacemi, ke kterým doplňuje výchozí hodnoty.
/// </summary>	
public class AnnotationsWithDefaultsEntityCacheOptionsGenerator : AnnotationsEntityCacheOptionsGenerator
{
	private readonly TimeSpan? _absoluteExpiration;
	private readonly TimeSpan? _slidingExpiration;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsWithDefaultsEntityCacheOptionsGenerator(IAnnotationsEntityCacheOptionsGeneratorStorage annotationsEntityCacheOptionsGeneratorStorage, IDbContext dbContext, INavigationTargetService navigationTargetTypeService, AnnotationsWithDefaultsEntityCacheOptionsGeneratorOptions options) : base(annotationsEntityCacheOptionsGeneratorStorage, dbContext, navigationTargetTypeService)
	{
		_absoluteExpiration = options.AbsoluteExpiration;
		_slidingExpiration = options.SlidingExpiration;
	}

	/// <inheritdoc />
	protected override CacheOptions GetCacheOptions(IReadOnlyEntityType entityType)
	{
		CacheOptions result = base.GetCacheOptions(entityType);

		if (result == null)
		{
			result = new CacheOptions();
		}

		if (result.AbsoluteExpiration == null)
		{
			result.AbsoluteExpiration = _absoluteExpiration;
		}

		if (result.SlidingExpiration == null)
		{
			result.SlidingExpiration = _slidingExpiration;
		}

		return result;
	}
}
