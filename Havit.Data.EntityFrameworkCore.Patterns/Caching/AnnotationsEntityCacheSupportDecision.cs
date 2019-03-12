using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
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
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DbContext je registrován scoped, proto se této factory popsaná issue týká.
	/// Z DbContextu jen čteme metadata (ta jsou pro každý DbContext stejná), issue tedy nemá žádný dopad.
	/// </remarks>	
	public class AnnotationsEntityCacheSupportDecision : IEntityCacheSupportDecision
	{
		private readonly Lazy<Dictionary<Type, bool>> shouldCacheEntities;
		private readonly Lazy<Dictionary<Type, bool>> shouldCacheAllKeys;
        private readonly Lazy<Dictionary<TypePropertyName, Type>> collectionTargetTypes;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public AnnotationsEntityCacheSupportDecision(IDbContextFactory dbContextFactory)
		{
			shouldCacheEntities = GetLazyDictionary(dbContextFactory, entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
			shouldCacheAllKeys = GetLazyDictionary(dbContextFactory, entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
            collectionTargetTypes = GetLazyCollectionTargetTypesDictionary(dbContextFactory);
        }

		/// <inheritdoc />
		public virtual bool ShouldCacheEntityType(Type entityType)
		{
            return GetValueFromDictionary(shouldCacheEntities.Value, entityType);
		}

		/// <inheritdoc />
		public virtual bool ShouldCacheEntity(object entity)
		{
            return ShouldCacheEntityType(entity.GetType());
		}

        /// <inheritdoc />
        public virtual bool ShouldCacheEntityTypeCollection(Type entityType, string propertyName)
        {
            if (collectionTargetTypes.Value.TryGetValue(new TypePropertyName(entityType, propertyName), out var targetType))
            {
                return ShouldCacheEntityType(targetType);
            }
            else
            {
                throw new InvalidOperationException($"Cannot resolve target type for {entityType.Name}.{propertyName}.");
            }
        }

        /// <inheritdoc />
        public virtual bool ShouldCacheEntityCollection(object entity, string propertyName)
		{
            return ShouldCacheEntityTypeCollection(entity.GetType(), propertyName);
        }

        /// <inheritdoc />
        public virtual bool ShouldCacheAllKeys(Type entityType)
		{
			return GetValueFromDictionary(shouldCacheAllKeys.Value, entityType);
		}

		private Lazy<Dictionary<Type, TResult>> GetLazyDictionary<TResult>(IDbContextFactory dbContextFactory, Func<IEntityType, TResult> valueFunc)
		{
			return new Lazy<Dictionary<Type, TResult>>(() =>
			{
				Dictionary<Type, TResult> result = null;
				dbContextFactory.ExecuteAction(dbContext =>
				{
					result = dbContext.Model.GetApplicationEntityTypes().ToDictionary(
						entityType => entityType.ClrType,
						entityType => valueFunc(entityType));
				});
				return result;
			}, LazyThreadSafetyMode.PublicationOnly);
		}

        private Lazy<Dictionary<TypePropertyName, Type>> GetLazyCollectionTargetTypesDictionary(IDbContextFactory dbContextFactory)
        {
            return new Lazy<Dictionary<TypePropertyName, Type>>(() =>
            {
                Dictionary<TypePropertyName, Type> result = null;
                dbContextFactory.ExecuteAction(dbContext =>
                {
                    result = dbContext.Model.GetApplicationEntityTypes()
                    .SelectMany(entityType => entityType.GetNavigations())
                    .Where(navigation => navigation.IsCollection())
                    .ToDictionary(
                        navigation => new TypePropertyName(navigation.DeclaringEntityType.ClrType, navigation.Name),
                        navigation => navigation.GetTargetType().ClrType);
                });
                return result;
            }, LazyThreadSafetyMode.PublicationOnly);
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
