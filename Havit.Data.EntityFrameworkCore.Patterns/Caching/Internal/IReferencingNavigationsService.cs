using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje seznam kolekcí a one-to-one referencí referencující danou entitu.
/// </summary>
public interface IReferencingNavigationsService
{
	/// <summary>
	/// Vrací seznam kolekcí a one-to-one referencí referencující danou entitu. Určeno pro invalidaci cachování.
	/// </summary>
	List<ReferencingNavigation> GetReferencingNavigations(IEntityType entityType);
}