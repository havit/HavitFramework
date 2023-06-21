using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Mapování kolekcí k typům entit.
/// </summary>
public interface ICollectionTargetTypeStorage
{
	/// <summary>
	/// Mapování kolekcí k typům entit.
	/// </summary>
	Dictionary<TypePropertyName, Type> Value { get; set; }
}
