using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.EntityFrameworkCore.Attributes;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Conventions
{
	/// <summary>
	/// Konvence nastaví hodnoty z CacheAttribute do anotací.
	/// </summary>
	public class CacheAttributeToAnnotationConvention : IModelConvention
	{
		/// <summary>
		/// Název anotace určující, zda je na povoleno cachování entity.
		/// </summary>
		public const string CacheEntitiesAnnotationName = "Caching-EntitiesEnabled";

		/// <summary>
		/// Název anotace určující, zda je na povoleno cachování AllKeys.
		/// </summary>
		public const string CacheAllKeysAnnotationName = "Caching-AllKeysEnabled";

		/// <summary>
		/// Název anotace určující nastavení sliding expirace.
		/// </summary>
		public const string SlidingExpirationAnnotationName = "Caching-SlidingExpiration";

		/// <summary>
		/// Název anotace určující nastavení absolute expirace.
		/// </summary>
		public const string AbsoluteExpirationAnnotationName = "Caching-AbsoluteExpiration";

		/// <summary>
		/// Název anotace určující nastavení priority položek v cache.
		/// </summary>
		public const string PriorityAnnotationName = "Caching-Priority";

		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			var entityTypesWithCacheAttribute =
				(from entityType in modelBuilder.Model
					.GetApplicationEntityTypes()
					.WhereNotConventionSuppressed(typeof(CacheAttributeToAnnotationConvention)) // testujeme entity types
					let cacheAttribute = entityType.ClrType.GetCustomAttributes(false).OfType<CacheAttribute>().SingleOrDefault()
					where cacheAttribute != null
					select new { EntityType = entityType, CacheAttribute = cacheAttribute }
				).ToList();

			foreach (var item in entityTypesWithCacheAttribute)
			{
				if (item.CacheAttribute.CacheEntities)
				{
					item.EntityType.SetAnnotation(CacheEntitiesAnnotationName, true);
				}

				if (item.CacheAttribute.CacheAllKeys)
				{
					item.EntityType.SetAnnotation(CacheAllKeysAnnotationName, true);
				}

				if (item.CacheAttribute.AbsoluteExpirationSeconds != 0)
				{
					item.EntityType.SetAnnotation(AbsoluteExpirationAnnotationName, item.CacheAttribute.AbsoluteExpirationSeconds);
				}

				if (item.CacheAttribute.SlidingExpirationSeconds != 0)
				{
					item.EntityType.SetAnnotation(AbsoluteExpirationAnnotationName, item.CacheAttribute.SlidingExpirationSeconds);
				}

				if (item.CacheAttribute.Priority != CacheItemPriority.Normal)
				{
					item.EntityType.SetAnnotation(PriorityAnnotationName, item.CacheAttribute.Priority);
				}
			}
		}
	}
}
