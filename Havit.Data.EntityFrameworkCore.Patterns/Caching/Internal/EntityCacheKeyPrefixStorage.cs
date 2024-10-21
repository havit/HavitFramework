namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <inheritdoc />
public class EntityCacheKeyPrefixStorage : IEntityCacheKeyPrefixStorage
{
	/// <inheritdoc />
	public FrozenDictionary<Type, string> Value { get; set; }
}
