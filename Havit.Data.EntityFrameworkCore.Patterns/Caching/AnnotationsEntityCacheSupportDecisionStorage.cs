using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Úložiště informací k rozhodnutí o cachování entit.
/// </summary>
public class AnnotationsEntityCacheSupportDecisionStorage : IAnnotationsEntityCacheSupportDecisionStorage
{
	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat entity.
	/// </summary>
	public Dictionary<Type, bool> ShouldCacheEntities { get; set; }

	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat "all keys".
	/// </summary>
	public Dictionary<Type, bool> ShouldCacheAllKeys { get; set; }
}
