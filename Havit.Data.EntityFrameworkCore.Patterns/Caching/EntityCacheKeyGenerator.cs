using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Služba pro poskytnutí strinkových klíčů do cache.
	/// Do klíče generuje názvy typu.
	/// </summary>
	public class EntityCacheKeyGenerator : IEntityCacheKeyGenerator
	{
		/// <inheritdoc />
		public string GetAllKeysCacheKey(Type entityType)
		{
			return entityType.FullName + "|AllKeys";
		}

		/// <inheritdoc />
		public string GetEntityCacheKey(Type entityType, object key)
		{			
			return entityType.FullName + "|ID=" + key.ToString();
		}
	}
}
