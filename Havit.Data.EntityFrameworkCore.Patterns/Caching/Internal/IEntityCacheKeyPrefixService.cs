namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje prefix klíče v cache pro daný typ entity.
/// </summary>
public interface IEntityCacheKeyPrefixService
{
	/// <summary>
	/// Poskytuje prefix klíče v cache pro daný typ entity.
	/// </summary>
	string GetCacheKeyPrefix(Type entityType);
}
