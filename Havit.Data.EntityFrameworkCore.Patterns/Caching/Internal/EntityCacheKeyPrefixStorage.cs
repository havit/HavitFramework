namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class EntityCacheKeyPrefixStorage : IEntityCacheKeyPrefixStorage
{
	/// <inheritdoc />
	public required FrozenDictionary<Type, string> Value { get; init; }
}
