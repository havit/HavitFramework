using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
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
		private readonly Lazy<Dictionary<Type, PropertyInfo>> propertyInfos;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbEntityKeyAccessor(IServiceFactory<IDbContext> dbContextFactory)
		{
			// pro možnost použití jako singletonu pro všechny případy používáme LazyThreadSafetyMode.ExecutionAndPublication
			propertyInfos = new Lazy<Dictionary<Type, PropertyInfo>>(() =>
			{
				Dictionary<Type, PropertyInfo> result = null;
				dbContextFactory.ExecuteAction(dbContext =>
				{
					result = dbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false).ToDictionary(entityType => entityType.ClrType, entityType => entityType.FindPrimaryKey().Properties.Single().PropertyInfo);
				});
				return result;
			}, LazyThreadSafetyMode.PublicationOnly);
		}

		/// <summary>
		/// Vrátí hodnotu primárního klíče entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public object GetEntityKey(object entity)
		{
			Contract.Requires(entity != null);
			return GetPropertyInto(entity.GetType()).GetValue(entity);
		}

		/// <summary>
		/// Vrátí název vlastnosti, která je primárním klíčem.
		/// </summary>
		public string GetEntityKeyPropertyName(Type entityType)
		{
			return GetPropertyInto(entityType).Name;
		}

		private PropertyInfo GetPropertyInto(Type entityType)
		{
			if (propertyInfos.Value.TryGetValue(entityType, out PropertyInfo propertyInfo))
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
