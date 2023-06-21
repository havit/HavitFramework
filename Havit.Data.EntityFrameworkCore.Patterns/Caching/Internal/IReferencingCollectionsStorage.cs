using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Seznam kolekcí referencující danou entitu.
/// </summary>
public interface IReferencingCollectionsStorage
{
	/// <summary>
	/// Seznam kolekcí referencující danou entitu.
	/// </summary>
	public Dictionary<Type, List<ReferencingCollection>> Value { get; set; }
}