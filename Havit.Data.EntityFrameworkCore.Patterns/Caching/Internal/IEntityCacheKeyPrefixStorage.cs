namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Mapování typu entity na prefix klíče v cache.
/// </summary>
public interface IEntityCacheKeyPrefixStorage
{
	/// <summary>
	/// Mapování typu entity na prefix klíče v cache.
	/// </summary>
	Dictionary<Type, string> Value { get; set; }
}
