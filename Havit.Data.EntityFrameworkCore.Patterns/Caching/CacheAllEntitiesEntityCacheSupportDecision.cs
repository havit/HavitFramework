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
		public bool ShouldCacheEntityType(Type entityType)
		{
			return true;
		}

		/// <inheritdoc/> 
		public bool ShouldCacheEntity(object entity)
		{
			return true;
		}

        /// <inheritdoc/> 
        public bool ShouldCacheEntityTypeCollection(Type entityType, string propertyName)
        {
            return true;
        }

        /// <inheritdoc/> 
        public bool ShouldCacheEntityCollection(object entity, string propertyName)
		{
			return true;
		}

		/// <inheritdoc/> 
		public bool ShouldCacheAllKeys(Type entityType)
		{
			return true;
		}
    }
}
