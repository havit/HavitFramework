using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Seznam kolekcí a one-to-one referencí referencující danou entitu.
/// </summary>
public class ReferencingNavigationsStorage : IReferencingNavigationsStorage
{
	/// <summary>
	/// Seznam kolekcí referencující danou entitu.
	/// </summary>
	public FrozenDictionary<IEntityType, List<ReferencingNavigation>> Value { get; set; }
}
