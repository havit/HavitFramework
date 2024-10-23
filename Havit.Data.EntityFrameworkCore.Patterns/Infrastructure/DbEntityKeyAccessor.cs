using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

/// <summary>
/// Služba pro získávání primárního klíče modelových objektů.
/// </summary>
public class DbEntityKeyAccessor : IEntityKeyAccessor
{
	private readonly IDbEntityKeyAccessorStorage _dbEntityKeyAccessorStorage;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbEntityKeyAccessor(IDbEntityKeyAccessorStorage dbEntityKeyAccessorStorage)
	{
		_dbEntityKeyAccessorStorage = dbEntityKeyAccessorStorage;
	}

	/// <summary>
	/// Vrátí hodnotu primárního klíče entity.
	/// </summary>
	/// <param name="entity">Entita.</param>
	public object[] GetEntityKeyValues(object entity)
	{
		// TODO EF Core 9: Velmi často se používá na výsledek metody .Single(), pokud uděláme dedikovanou metodu, odstraníme alokaci pole pomocí .ToArray().
		return GetDbEntityKeyAccessorItem(entity.GetType()).PropertyInfos.Select(propertyInfo => propertyInfo.GetValue(entity)).ToArray();
	}

	/// <summary>
	/// Vrátí název vlastnosti, která je primárním klíčem.
	/// </summary>
	public string[] GetEntityKeyPropertyNames(Type entityType)
	{
		return GetDbEntityKeyAccessorItem(entityType).PropertyNames;
	}

	private DbEntityKeyAccessorItem GetDbEntityKeyAccessorItem(Type entityType)
	{
		if (_dbEntityKeyAccessorStorage.Value.TryGetValue(entityType, out DbEntityKeyAccessorItem item))
		{
			return item;
		}
		else
		{
			throw new InvalidOperationException(String.Format("Type {0} is not a supported type.", entityType.FullName));
		}
	}
}
