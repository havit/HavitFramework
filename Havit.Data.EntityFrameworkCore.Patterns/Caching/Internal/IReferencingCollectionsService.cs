using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje seznam kolekcí referencující danou entitu.
/// </summary>
public interface IReferencingCollectionsService
{
	/// <summary>
	/// Vrací seznam kolekcí referencující danou entitu vš. funkce pro vrácení hodnoty cizího klíče vůči takové entitě. Určeno pro invalidaci cachování.
	/// </summary>
	List<ReferencingCollection> GetReferencingCollections(System.Type entityType);
}