using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Seznam kolekcí a one-to-one referencí referencující danou entitu.
/// </summary>
public interface IReferencingNavigationsStorage
{
	/// <summary>
	/// Seznam kolekcí a one-to-one referencí referencující danou entitu.
	/// </summary>
	FrozenDictionary<IEntityType, List<ReferencingNavigation>> Value { get; }
}