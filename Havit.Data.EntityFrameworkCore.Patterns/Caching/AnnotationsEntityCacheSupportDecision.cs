using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Services;
using Havit.Services.Caching;
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
		private readonly Lazy<Dictionary<Type, bool>> shouldCacheEntities;
		private readonly Lazy<Dictionary<Type, bool>> shouldCacheAllKeys;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AnnotationsEntityCacheSupportDecision(IServiceFactory<IDbContext> dbContextFactory)
		{
			shouldCacheEntities = GetLazyDictionary(dbContextFactory, entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
			shouldCacheAllKeys = GetLazyDictionary(dbContextFactory, entityType => ((bool?)(entityType.FindAnnotation(CacheAttributeToAnnotationConvention.CacheEntitiesAnnotationName)?.Value)).GetValueOrDefault(false));
		}

		/// <inheritdoc />
		public virtual bool ShouldCacheEntity<TEntity>()
			where TEntity : class
		{
			return GetValueFromDictionary(shouldCacheEntities.Value, typeof(TEntity));
		}

		/// <inheritdoc />
		public virtual bool ShouldCacheEntity<TEntity>(TEntity entity)
			where TEntity : class
		{
			return ShouldCacheEntity<TEntity>();
		}

		/// <inheritdoc />
		public bool ShouldCacheAllKeys<TEntity>()
			where TEntity : class
		{
			return GetValueFromDictionary(shouldCacheAllKeys.Value, typeof(TEntity));
		}

		private Lazy<Dictionary<Type, TResult>> GetLazyDictionary<TResult>(IServiceFactory<IDbContext> dbContextFactory, Func<IEntityType, TResult> valueFunc)
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
