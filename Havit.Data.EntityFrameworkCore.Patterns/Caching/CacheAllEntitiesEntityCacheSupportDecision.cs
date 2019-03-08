using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Rozhoduje, že mohou být cachovány všechny entity.
	/// </summary>
	public sealed class CacheAllEntitiesEntityCacheSupportDecision : IEntityCacheSupportDecision
	{
		/// <inheritdoc/> 
		public bool ShouldCacheEntity<TEntity>()
			where TEntity : class
		{
			return true;
		}

		/// <inheritdoc/> 
		public bool ShouldCacheEntity<TEntity>(TEntity entity)
			where TEntity : class
		{
			return true;
		}

		/// <inheritdoc/> 
		public bool ShouldCacheCollection<TEntity>(TEntity entity, string propertyName)
			where TEntity : class
		{
			return true;
		}

		/// <inheritdoc/> 
		public bool ShouldCacheAllKeys<TEntity>()
			where TEntity : class
		{
			return true;
		}
	}
}
