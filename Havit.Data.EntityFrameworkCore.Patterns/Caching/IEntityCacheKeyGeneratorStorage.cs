using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Mapování typu entity na klíč v cache.
/// </summary>
public interface IEntityCacheKeyGeneratorStorage
{
	/// <summary>
	/// Mapování typu entity na klíč v cache.
	/// </summary>
	Dictionary<Type, string> Value { get; set; }
}
