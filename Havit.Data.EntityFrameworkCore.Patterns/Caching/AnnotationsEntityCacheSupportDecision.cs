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
		public virtual bool ShouldCacheEntity<TEntity>()
			where TEntity : class
		{
            return ShouldCacheEntityInternal(typeof(TEntity));
		}

		/// <inheritdoc />
		public virtual bool ShouldCacheEntity<TEntity>(TEntity entity)
			where TEntity : class
		{
            return ShouldCacheEntityInternal(typeof(TEntity));
		}
        
		/// <inheritdoc />
		public bool ShouldCacheCollection<TEntity>(TEntity entity, string propertyName)
			where TEntity : class
		{
            if (collectionTargetTypes.Value.TryGetValue(new TypePropertyName(typeof(TEntity), propertyName), out var targetType))
            {
                return ShouldCacheEntityInternal(targetType);
            }
            else
            {
                throw new InvalidOperationException($"Cannot resolve target type for {typeof(TEntity).Name}.{propertyName}.");
            }
		}

        /// <summary>
		/// Vrací true, pokud může být entita daného typu cachována.
        /// </summary>
        protected bool ShouldCacheEntityInternal(Type targetType)
        {
            return GetValueFromDictionary(shouldCacheEntities.Value, targetType);
        }

        /// <inheritdoc />
        public bool ShouldCacheAllKeys<TEntity>()
			where TEntity : class
		{
			return GetValueFromDictionary(shouldCacheAllKeys.Value, typeof(TEntity));
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
