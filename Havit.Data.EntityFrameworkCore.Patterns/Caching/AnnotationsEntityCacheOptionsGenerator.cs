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
	public class AnnotationsEntityCacheOptionsGenerator : IEntityCacheOptionsGenerator
	{
		private readonly Lazy<Dictionary<Type, CacheOptions>> cacheOptionsDictionary;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AnnotationsEntityCacheOptionsGenerator(IServiceFactory<IDbContext> dbContextFactory)
		{
			cacheOptionsDictionary = GetLazyDictionary(dbContextFactory);
		}

		/// <inheritdoc />
		public CacheOptions GetEntityCacheOptions<TEntity>(TEntity entity)
			where TEntity : class
		{
			return GetValueFromDictionary(cacheOptionsDictionary.Value, typeof(TEntity));
		}

		/// <inheritdoc />
		public CacheOptions GetAllKeysCacheOptions<TEntity>()
			where TEntity : class
		{
			return GetValueFromDictionary(cacheOptionsDictionary.Value, typeof(TEntity));
		}

		private Lazy<Dictionary<Type, CacheOptions>> GetLazyDictionary(IServiceFactory<IDbContext> dbContextFactory)
		{
			return new Lazy<Dictionary<Type, CacheOptions>>(() =>
			{
				Dictionary<Type, CacheOptions> result = null;
				dbContextFactory.ExecuteAction(dbContext =>
				{
					result = dbContext.Model.GetApplicationEntityTypes().ToDictionary(
						entityType => entityType.ClrType,
						entityType =>
						{
							var options = GetCacheOptions(entityType);
							options.Freeze();
							return options;
						});
				});
				return result;
			});
		}

		/// <summary>
		/// Vrací cache options pro danou entitu.
		/// Neočekává se sdílená instance přes různé typy. CacheOptions jsou následně uzamčeny pro změnu.
		/// </summary>
		protected virtual CacheOptions GetCacheOptions(IEntityType entityType)
		{
			int? absoluteExpiration = (int?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.AbsoluteExpirationAnnotationName)?.Value;
			int? slidingExpiration = (int?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.SlidingExpirationAnnotationName)?.Value;
			Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority? priority = (Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority?)entityType.FindAnnotation(CacheAttributeToAnnotationConvention.PriorityAnnotationName)?.Value;

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

		private Havit.Services.Caching.CacheItemPriority GetPriority(Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority? priority)
		{
			switch (priority)
			{
				case null:
				case Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority.Normal:
					return Havit.Services.Caching.CacheItemPriority.Normal;

				case Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority.Low:
					return Havit.Services.Caching.CacheItemPriority.Low;

				case Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority.High:
					return Havit.Services.Caching.CacheItemPriority.High;

				case Havit.Data.EntityFrameworkCore.Abstractions.Attributes.CacheItemPriority.NotRemovable:
					return Havit.Services.Caching.CacheItemPriority.NotRemovable;

				default:
					throw new InvalidOperationException(priority.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private CacheOptions GetValueFromDictionary(Dictionary<Type, CacheOptions> valuesDictionary, Type type)
		{
			if (valuesDictionary.TryGetValue(type, out CacheOptions result))
			{
				return result;
			}
			return null;
		}

	}
}
