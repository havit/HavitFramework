namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Sestaví (I)EntityCacheKeyPrefixStorage.
/// </summary>
public interface IEntityCacheKeyPrefixStorageBuilder
{
	/// <summary>
	/// Sestaví (I)EntityCacheKeyPrefixStorage.
	/// </summary>
	IEntityCacheKeyPrefixStorage Build();
}