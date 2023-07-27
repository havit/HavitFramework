using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Navigace (kolekce nebo one-to-one reference).
/// </summary>
public class ReferencingNavigation
{
	/// <summary>
	/// Typ, který obsahuje vlastnost typu kolekce nebo one-to-one reference.
	/// </summary>
	public required Type EntityType { get; init; }

	/// <summary>
	/// Název vlastnosti, která je kolekcí nebo one-to-one referencí.
	/// </summary>
	public required string NavigationPropertyName { get; init; }

	/// <summary>
	/// Vlastnost cizího klíče v EF Core modelu.
	/// Nejde o vlastnost která náleži třídě EntityType, ale ve třídě, ve které je cizí klíč, který je protistranou navigace v EntityType.
	/// </summary>
	public required IProperty SourceEntityForeignKeyProperty { get; init; }
}
