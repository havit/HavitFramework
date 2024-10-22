using Havit.Data.Patterns.Infrastructure;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Služba pro získávání primárního klíče modelových objektů.
/// </summary>
public class DbEntityKeyAccessor<TEntity, TKey> : IEntityKeyAccessor<TEntity, TKey>
	where TEntity : class
{
	private readonly IEntityKeyAccessor _entityKeyAccessor;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbEntityKeyAccessor(IEntityKeyAccessor entityKeyAccessor)
	{
		_entityKeyAccessor = entityKeyAccessor;
	}

	/// <summary>
	/// Vrátí hodnotu primárního klíče entity.
	/// </summary>
	/// <param name="entity">Entita.</param>
	public TKey GetEntityKeyValue(TEntity entity)
	{
		Contract.Requires(entity != null);

		return (TKey)_entityKeyAccessor.GetEntityKeyValues(entity).Single();
	}

	/// <summary>
	/// Vrátí název vlastnosti, která je primárním klíčem.
	/// </summary>
	public string GetEntityKeyPropertyName()
	{
		return _entityKeyAccessor.GetEntityKeyPropertyNames(typeof(TEntity)).Single();
	}

}
