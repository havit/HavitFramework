using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;
using Havit.Services;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Služba pro získávání primárního klíče modelových objektů.
/// </summary>
public class DbEntityKeyAccessor<TEntity, TKey> : IEntityKeyAccessor<TEntity, TKey>
	where TEntity : class
{
	private readonly IEntityKeyAccessor entityKeyAccessor;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbEntityKeyAccessor(IEntityKeyAccessor entityKeyAccessor)
	{
		this.entityKeyAccessor = entityKeyAccessor;
	}

	/// <summary>
	/// Vrátí hodnotu primárního klíče entity.
	/// </summary>
	/// <param name="entity">Entita.</param>
	public TKey GetEntityKeyValue(TEntity entity)
	{
		Contract.Requires(entity != null);

		return (TKey)entityKeyAccessor.GetEntityKeyValues(entity).Single();
	}

	/// <summary>
	/// Vrátí název vlastnosti, která je primárním klíčem.
	/// </summary>
	public string GetEntityKeyPropertyName()
	{
		return entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TEntity)).Single();
	}

}
