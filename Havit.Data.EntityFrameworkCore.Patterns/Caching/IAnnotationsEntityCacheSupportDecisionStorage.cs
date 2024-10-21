using System.Collections.Frozen;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Úložiště informací k rozhodnutí o cachování entit.
/// </summary>
public interface IAnnotationsEntityCacheSupportDecisionStorage
{
	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat entity.
	/// </summary>
	FrozenDictionary<Type, bool> ShouldCacheEntities { get; set; }

	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat "all keys".
	/// </summary>
	FrozenDictionary<Type, bool> ShouldCacheAllKeys { get; set; }
}
