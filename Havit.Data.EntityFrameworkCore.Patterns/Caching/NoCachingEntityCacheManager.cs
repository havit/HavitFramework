using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Cache manager, který nic necachuje a nic nečte z cache.
	/// Nemá žádné závislosti.
	/// </summary>
	public sealed class NoCachingEntityCacheManager : IEntityCacheManager
	{
		/// <summary>
		/// Nic nedělá, nehledá v cache.
		/// Vrací vždy false.
		/// </summary>
		public bool TryGetEntity<TEntity>(object id, out TEntity entity)
			where TEntity : class
		{
			entity = null;
			return false;
		}

		/// <summary>
		/// Nic nedělá, neinvaliduje.
		/// </summary>
		public void InvalidateEntity(ChangeType changeType, object entity)
		{
			// NOOP
		}

		/// <summary>
		/// Nic nedělá, neukládá do cache.
		/// </summary>
		public void StoreEntity<TEntity>(TEntity entity)
			where TEntity : class
		{
			// NOOP
		}

		/// <summary>
		/// Nic nedělá, nehledá v cache.
		/// Vrací vždy false.
		/// </summary>
		public bool TryGetCollection<TEntity, TPropertyItem>(TEntity entityToLoad, string propertyName)
			where TEntity : class
			where TPropertyItem : class
		{
			return false;
		}

		/// <summary>
		/// Nic nedělá, neukládá do cache.
		/// </summary>
		public void StoreCollection<TEntity, TPropertyItem>(TEntity entity, string propertyName)
			where TEntity : class
			where TPropertyItem : class
		{
			// NOOP
		}

		/// <summary>
		/// Nic nedělá, nehledá v cache.
		/// Vrací vždy false.
		/// </summary>
		public bool TryGetAllKeys<TEntity>(out object keys)
			where TEntity : class
		{
			keys = null;
			return false;
		}

		/// <summary>
		/// Nic nedělá, neukládá do cache.
		/// </summary>
		public void StoreAllKeys<TEntity>(object keys)
			where TEntity : class
		{
			// NOOP
		}
	}
}
