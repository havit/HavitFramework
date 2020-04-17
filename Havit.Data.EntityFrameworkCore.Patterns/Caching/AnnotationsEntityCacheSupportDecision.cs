using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Services;
using Havit.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Výchozí strategie definující, zda může být entita cachována. Řídí se anotacemi.
	/// </summary>
	public class AnnotationsEntityCacheSupportDecision : IEntityCacheSupportDecision
	{
		private readonly IAnnotationsEntityCacheSupportDecisionStorage annotationsEntityCacheSupportDecisionStorage;
		private readonly IDbContext dbContext;
		private readonly ICollectionTargetTypeService collectionTargetTypeService;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public AnnotationsEntityCacheSupportDecision(IAnnotationsEntityCacheSupportDecisionStorage annotationsEntityCacheSupportDecisionStorage, IDbContext dbContext, ICollectionTargetTypeService collectionTargetTypeService)
		{
			this.annotationsEntityCacheSupportDecisionStorage = annotationsEntityCacheSupportDecisionStorage;
			this.dbContext = dbContext;
			this.collectionTargetTypeService = collectionTargetTypeService;
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
        public virtual bool ShouldCacheEntityTypeCollection(Type entityType, string propertyName)
        {
            return ShouldCacheEntityType(collectionTargetTypeService.GetCollectionTargetType(entityType, propertyName));
        }

        /// <inheritdoc />
        public virtual bool ShouldCacheEntityCollection(object entity, string propertyName)
		{
            return ShouldCacheEntityTypeCollection(entity.GetType(), propertyName);
        }

        /// <inheritdoc />
        public virtual bool ShouldCacheAllKeys(Type entityType)
		{
			return GetValueFromDictionary(GetShouldCacheAllKeysDictionary(), entityType);
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Dictionary<Type, bool> GetShouldCacheEntitiesDictionary()
		{
			if (annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities == null)
			{
				lock (annotationsEntityCacheSupportDecisionStorage)
				{
					if (annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities == null)
					{
						annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities = dbContext.Model.GetApplicationEntityTypes().ToDictionary(
							entityType => entityType.ClrType,
							entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheAllKeysAnnotationName)?.Value)).GetValueOrDefault(false));
					}
				}
			}
			return annotationsEntityCacheSupportDecisionStorage.ShouldCacheEntities;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Dictionary<Type, bool> GetShouldCacheAllKeysDictionary()
		{
			if (annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys == null)
			{
				lock (annotationsEntityCacheSupportDecisionStorage)
				{
					if (annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys == null)
					{
						annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys = dbContext.Model.GetApplicationEntityTypes().ToDictionary(
							entityType => entityType.ClrType,
							entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
					}
				}
			}
			return annotationsEntityCacheSupportDecisionStorage.ShouldCacheAllKeys;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetValueFromDictionary(Dictionary<Type, bool> valuesDictionary, Type type)
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
}
