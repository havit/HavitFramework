namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Úložiště informací k rozhodnutí o cachování entit.
/// </summary>
public class AnnotationsEntityCacheSupportDecisionStorage : IAnnotationsEntityCacheSupportDecisionStorage
{
	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat entity.
	/// </summary>
	public required FrozenDictionary<Type, bool> ShouldCacheEntities { get; init; }

	/// <summary>
	/// Indikuje ke každému typu, zda má cachovat "all keys".
	/// </summary>
	public required FrozenDictionary<Type, bool> ShouldCacheAllKeys { get; init; }
}
