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
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DbContext je registrován scoped, proto se této factory popsaná issue týká.
	/// Z DbContextu jen čteme metadata (ta jsou pro každý DbContext stejná), issue tedy nemá žádný dopad.
	/// </remarks>
	public class DbEntityKeyAccessor : IEntityKeyAccessor
	{
		private readonly Lazy<Dictionary<Type, PropertyInfo[]>> propertyInfos;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbEntityKeyAccessor(IDbContextFactory dbContextFactory)
		{
			// pro možnost použití jako singletonu pro všechny případy používáme LazyThreadSafetyMode.ExecutionAndPublication
			propertyInfos = new Lazy<Dictionary<Type, PropertyInfo[]>>(() =>
			{
				Dictionary<Type, PropertyInfo[]> result = null;
				dbContextFactory.ExecuteAction(dbContext =>
				{
					result = dbContext.Model.GetApplicationEntityTypes().ToDictionary(entityType => entityType.ClrType, entityType => entityType.FindPrimaryKey().Properties.Select(property => property.PropertyInfo).ToArray());
				});
				return result;
			}, LazyThreadSafetyMode.PublicationOnly);
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
			if (propertyInfos.Value.TryGetValue(entityType, out PropertyInfo[] propertyInfo))
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
