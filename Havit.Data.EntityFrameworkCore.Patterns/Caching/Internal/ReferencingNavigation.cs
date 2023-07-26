using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Navigace (kolekce nebo one-to-one reference).
/// </summary>
public class ReferencingNavigation
{
	/// <summary>
	/// Typ, který obsahuje vlastnost typu kolekce nebo one-to-one reference.
	/// </summary>
	public Type EntityType { get; set; }

	/// <summary>
	/// Název vlastnosti, která je kolekcí nebo one-to-one referencí.
	/// </summary>
	public string NavigationPropertyName { get; set; }

	// TODO: Odstranit, nahradit hodnotami v Change[s].
	/// <summary>
	/// Funkce, která vrátí hodnotu cizího klíče směřujícího k dané kolekci.
	/// </summary>
	public Func<IDbContext, object, object> GetForeignKeyValue { get; set; }
}
