using System;

namespace Havit.Data.Patterns.Infrastructure;

/// <summary>
/// Služba pro získávání primárního klíče modelových objektů.
/// </summary>
public interface IEntityKeyAccessor
{
	/// <summary>
	/// Vrátí hodnotu primárního klíče entity.
	/// </summary>
	/// <param name="entity">Entita.</param>
	object[] GetEntityKeyValues(object entity);

	/// <summary>
	/// Vrátí název vlastnosti, která je primárním klíčem.
	/// </summary>
	string[] GetEntityKeyPropertyNames(Type entityType);
}