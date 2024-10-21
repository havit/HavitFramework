﻿using System.Runtime.CompilerServices;
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
	private readonly IAnnotationsEntityCacheOptionsGeneratorStorage annotationsEntityCacheOptionsGeneratorStorage;
	private readonly IDbContext dbContext;
	private readonly INavigationTargetService navigationTargetService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsEntityCacheOptionsGenerator(IAnnotationsEntityCacheOptionsGeneratorStorage annotationsEntityCacheOptionsGeneratorStorage, IDbContext dbContext, INavigationTargetService navigationTargetService)
	{
		this.annotationsEntityCacheOptionsGeneratorStorage = annotationsEntityCacheOptionsGeneratorStorage;
		this.dbContext = dbContext;
		this.navigationTargetService = navigationTargetService;
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
		return GetValueForEntity(navigationTargetService.GetNavigationTarget(typeof(TEntity), propertyName).TargetClrType);
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
		if (annotationsEntityCacheOptionsGeneratorStorage.Value == null)
		{
			lock (annotationsEntityCacheOptionsGeneratorStorage)
			{
				if (annotationsEntityCacheOptionsGeneratorStorage.Value == null)
				{
					annotationsEntityCacheOptionsGeneratorStorage.Value = dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
						entityType => entityType.ClrType,
						entityType =>
						{
							var options = GetCacheOptions(entityType);
							options?.Freeze();
							return options;
						});
				}
			}
		}

		if (annotationsEntityCacheOptionsGeneratorStorage.Value.TryGetValue(type, out CacheOptions result))
		{
			return result;
		}

		return null;
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
