﻿using System.Runtime.CompilerServices;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Výchozí strategie definující, zda může být entita cachována. Řídí se anotacemi.
/// </summary>
public class AnnotationsEntityCacheSupportDecision : IEntityCacheSupportDecision
{
	private readonly IAnnotationsEntityCacheSupportDecisionStorage _annotationsEntityCacheSupportDecisionStorage;
	private readonly IDbContext _dbContext;
	private readonly INavigationTargetService _navigationTargetService;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AnnotationsEntityCacheSupportDecision(IAnnotationsEntityCacheSupportDecisionStorage annotationsEntityCacheSupportDecisionStorage, IDbContext dbContext, INavigationTargetService navigationTargetService)
	{
		_annotationsEntityCacheSupportDecisionStorage = annotationsEntityCacheSupportDecisionStorage;
		_dbContext = dbContext;
		_navigationTargetService = navigationTargetService;
	}

	/// <inheritdoc />
	public virtual bool ShouldCacheEntityType(Type entityType)
	{
		return GetValueFromDictionary(GetShouldCacheEntitiesDictionary(), entityType);
	}

	/// <inheritdoc />
	public virtual bool ShouldCacheEntity(object entity)
	{
		return ShouldCacheEntityType(entity.GetType());
	}

	/// <inheritdoc />
	public virtual bool ShouldCacheEntityTypeNavigation(Type entityType, string propertyName)
	{
		// 1) kolekce one-to-many (např. Invoice.Items)
		// - Kolekce Invoice.Items má být cachována, pokud jsou cachovány InvoiceItems.
		// - Při vybavování dat z cache pro Invoice.Items je důležité, aby byly v cache k dispozici InvoiceItems.

		// 2) kolekce one-to-many reprezentující dekomponovaný vztak many-to-many (např. User.Memberships)
		// - Je jen specifické použití prvního bodu, platí tedy totéž:
		// - Kolekce má být cachována, pokud jsou cachovány Memberships.
		// - Při vybavování dat z cache pro User.Memberships je důležité, aby byly v cache k dispozici Memberships.		

		// 3) kolekce many-to-many (např. User.Roles)
		// - Je jen specifické použití prvního bodu, platí tedy totéž.
		// - Jen je mezi entitami ještě "neviditelná" (SkipNavigation) entita.
		// - Při vybavování dat z cache pro User.Roles je důležité, aby byly v cache k dispozici Roles.
		// - SkipNavigation entitu v DbDataloaderu nějak konstruujeme...

		// 4) reference one-to-one (backreference)
		// - Opet platí, že je při vybavování dat z cache je důležité, aby byly v cache entity protistrany.

		return ShouldCacheEntityType(_navigationTargetService.GetNavigationTarget(entityType, propertyName).TargetClrType);
	}

	/// <inheritdoc />
	public virtual bool ShouldCacheEntityNavigation(object entity, string propertyName)
	{
		return ShouldCacheEntityTypeNavigation(entity.GetType(), propertyName);
	}

	/// <inheritdoc />
	public virtual bool ShouldCacheAllKeys(Type entityType)
	{
		return GetValueFromDictionary(GetShouldCacheAllKeysDictionary(), entityType);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private FrozenDictionary<Type, bool> GetShouldCacheEntitiesDictionary()
	{
		if (_annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities == null)
		{
			lock (_annotationsEntityCacheSupportDecisionStorage)
			{
				if (_annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities == null)
				{
					_annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities = _dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
						entityType => entityType.ClrType,
						entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheAllKeysAnnotationName)?.Value)).GetValueOrDefault(false));
				}
			}
		}
		return _annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private FrozenDictionary<Type, bool> GetShouldCacheAllKeysDictionary()
	{
		if (_annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys == null)
		{
			lock (_annotationsEntityCacheSupportDecisionStorage)
			{
				if (_annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys == null)
				{
					_annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys = _dbContext.Model.GetApplicationEntityTypes().ToFrozenDictionary(
						entityType => entityType.ClrType,
						entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
				}
			}
		}
		return _annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool GetValueFromDictionary(FrozenDictionary<Type, bool> valuesDictionary, Type type)
	{
		if (valuesDictionary.TryGetValue(type, out bool result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", type.FullName));
		}
	}

}
