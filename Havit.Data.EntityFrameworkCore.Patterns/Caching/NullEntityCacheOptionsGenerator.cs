using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Vrací vždy null. Pro použití tam, kde nepotřebujeme řešit CacheOptions, např. v unit testech.
/// </summary>
public class NullEntityCacheOptionsGenerator : IEntityCacheOptionsGenerator
{
	/// <inheritdoc />
	public CacheOptions GetEntityCacheOptions<TEntity>(TEntity entity)
		where TEntity : class
	{
		return null;
	}

	/// <inheritdoc />
	public CacheOptions GetNavigationCacheOptions<TEntity>(TEntity entity, string propertyName)
		where TEntity : class
	{
		return null;
	}

	/// <inheritdoc />
	public CacheOptions GetAllKeysCacheOptions<TEntity>()
		where TEntity : class
	{
		return null;
	}
}
