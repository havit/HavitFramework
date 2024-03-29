﻿namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Služba pro poskytnutí stringových klíčů do cache.
/// Pro distribuovanou invalidaci musí být klíče deterministické.
/// </summary>
public interface IEntityCacheKeyGenerator
{
	/// <summary>
	/// Vrací klíč pro cachování entity daného typu s daným klíčem.
	/// </summary>
	string GetEntityCacheKey(Type entityType, object key);

	/// <summary>
	/// Vrací klíč pro cachování prvků kolekce nebo one-to-one "back-reference" dané entity.
	/// </summary>
	string GetNavigationCacheKey(Type entityType, object key, string propertyName);

	/// <summary>
	/// Vrací klíč pro cachování klíčů všech entit daného typu.
	/// </summary>
	string GetAllKeysCacheKey(Type entityType);

}
