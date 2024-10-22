using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Výchozí strategie definující, zda může být entita cachována. Řídí se anotacemi.
/// </summary>
public class AnnotationsEntityCacheOptionsGenerator : IEntityCacheOptionsGenerator
{
	private readonly IAnnotationsEntityCacheOptionsGeneratorStorage _annotationsEntityCacheOptionsGeneratorStorage;
	private readonly INavigationTargetService _navigationTargetService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsEntityCacheOptionsGenerator(IAnnotationsEntityCacheOptionsGeneratorStorage annotationsEntityCacheOptionsGeneratorStorage, INavigationTargetService navigationTargetService)
	{
		_annotationsEntityCacheOptionsGeneratorStorage = annotationsEntityCacheOptionsGeneratorStorage;
		_navigationTargetService = navigationTargetService;
	}

	/// <inheritdoc />
	public CacheOptions GetEntityCacheOptions<TEntity>(TEntity entity)
		where TEntity : class
	{
		return GetValueForEntity(typeof(TEntity));
	}

	/// <inheritdoc />
	public CacheOptions GetNavigationCacheOptions<TEntity>(TEntity entity, string propertyName)
		where TEntity : class
	{
		return GetValueForEntity(_navigationTargetService.GetNavigationTarget(typeof(TEntity), propertyName).TargetClrType);
	}

	/// <inheritdoc />
	public CacheOptions GetAllKeysCacheOptions<TEntity>()
		where TEntity : class
	{
		return GetValueForEntity(typeof(TEntity));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private CacheOptions GetValueForEntity(Type type)
	{
		_annotationsEntityCacheOptionsGeneratorStorage.Value.TryGetValue(type, out CacheOptions result);
		return result;
	}
}
