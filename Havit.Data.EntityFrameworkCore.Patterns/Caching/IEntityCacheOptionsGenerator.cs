using Havit.Services.Caching;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro vytvoření cache options při přidání položky do cache.
/// Implementace mají umožnit např. prioritu cachování položek, nebo např. určení sliding expirace.
/// Použije se pouze pro entity, které se ukládají do cache, tj. netřeba implementačně řešit, že některé entity nejsou cachované.
/// </summary>
public interface IEntityCacheOptionsGenerator
{
	/// <summary>
	/// Vrací cache options pro cachování entity daného typu.
	/// </summary>
	CacheOptions GetEntityCacheOptions<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Vrací cache options pro cachování kolekce nebo one-to-one "back-reference" dané entity.
	/// </summary>
	CacheOptions GetNavigationCacheOptions<TEntity>(TEntity entity, string propertyName)
		where TEntity : class;

	/// <summary>
	/// Vrací cache options pro cachování všech klíčů entit daného typu.
	/// </summary>
	CacheOptions GetAllKeysCacheOptions<TEntity>()
		where TEntity : class;
}
