﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure
{
	/// <summary>
	/// Služba pro získávání primárního klíče modelových objektů.
	/// </summary>
	internal class DbEntityKeyAccessor<TEntity, TKey> : IEntityKeyAccessor<TEntity, TKey>
		where TEntity : class
	{
		private readonly Lazy<PropertyInfo> primaryKeyPropertyInfoLazy;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbEntityKeyAccessor(IDbContext dbContext)
		{
			// pro možnost použití jako singletonu pro všechny případy používáme LazyThreadSafetyMode.ExecutionAndPublication
			primaryKeyPropertyInfoLazy = new Lazy<PropertyInfo>(() => dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.Single().PropertyInfo, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		/// <summary>
		/// Vrátí hodnotu primárního klíče entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public TKey GetEntityKey(TEntity entity)
		{
			Contract.Requires(entity != null);

			return (TKey)primaryKeyPropertyInfoLazy.Value.GetValue(entity);
		}

		/// <summary>
		/// Vrátí název vlastnosti, která je primárním klíčem.
		/// </summary>
		public string GetEntityKeyPropertyName()
		{
			return primaryKeyPropertyInfoLazy.Value.Name;
		}

	}
}
