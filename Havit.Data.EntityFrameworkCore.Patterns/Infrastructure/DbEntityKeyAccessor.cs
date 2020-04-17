using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Havit.Data.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure
{
	/// <summary>
	/// Služba pro získávání primárního klíče modelových objektů.
	/// </summary>
	public class DbEntityKeyAccessor : IEntityKeyAccessor
	{
		private readonly IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage;
		private readonly IDbContext dbContext;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbEntityKeyAccessor(IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage, IDbContext dbContext)
		{
			// pro možnost použití jako singletonu pro všechny případy používáme LazyThreadSafetyMode.ExecutionAndPublication
			this.dbEntityKeyAccessorStorage = dbEntityKeyAccessorStorage;
			this.dbContext = dbContext;
		}

		/// <summary>
		/// Vrátí hodnotu primárního klíče entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public object[] GetEntityKeyValues(object entity)
		{
			Contract.Requires(entity != null);
			return GetPropertyInfos(entity.GetType()).Select(propertyInfo => propertyInfo.GetValue(entity)).ToArray();
		}

		/// <summary>
		/// Vrátí název vlastnosti, která je primárním klíčem.
		/// </summary>
		public string[] GetEntityKeyPropertyNames(Type entityType)
		{
			return GetPropertyInfos(entityType).Select(propertyInfo => propertyInfo.Name).ToArray();
		}

		private PropertyInfo[] GetPropertyInfos(Type entityType)
		{
			if (dbEntityKeyAccessorStorage.Value == null)
			{
				lock (dbEntityKeyAccessorStorage)
				{
					if (dbEntityKeyAccessorStorage.Value == null)
					{
						dbEntityKeyAccessorStorage.Value = dbContext.Model.GetApplicationEntityTypes().ToDictionary(entityType => entityType.ClrType, entityType => entityType.FindPrimaryKey().Properties.Select(property => property.PropertyInfo).ToArray());
					}
				}
			}

			if (dbEntityKeyAccessorStorage.Value.TryGetValue(entityType, out PropertyInfo[] propertyInfo))
			{
				return propertyInfo;
			}
			else
			{
				throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", entityType.FullName));
			}
		}
	}
}
