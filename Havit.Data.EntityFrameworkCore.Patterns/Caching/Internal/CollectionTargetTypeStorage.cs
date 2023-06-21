using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Mapování kolekcí k typům entit.
/// </summary>
public class CollectionTargetTypeStorage : ICollectionTargetTypeStorage
{
	/// <summary>
	/// Mapování kolekcí k typům entit.
	/// </summary>
	public Dictionary<TypePropertyName, Type> Value { get; set; }
}
