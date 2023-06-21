using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Třída pro poskytnutí klíčů pro definování závislostí v cache.
/// </summary>
public interface IEntityCacheDependencyKeyGenerator
{
	/// <summary>
	/// Vrací klíč pro definování závislosti na uložení dané entity.
	/// Pokud dojde k uložení entity daného typu s daným klíčem, jsou invalidovány všechny objekty závislé na tomto klíči.
	/// Klíč je v cache automaticky vytvořen (pokud není řečeno jinak).
	/// </summary>
	string GetSaveCacheDependencyKey(Type entityType, object key, bool ensureInCache = true);

	/// <summary>
	/// Vrací klíč pro definování závislosti na uložení jakékoliv entity daného typu.
	/// Pokud dojde k uložení jakékoliv entity daného typu, jsou invalidovány všechny objekty závislé na tomto klíči.
	/// Klíč je v cache automaticky vytvořen (pokud není řečeno jinak).
	/// </summary>
	string GetAnySaveCacheDependencyKey(Type entityType, bool ensureInCache = true);
}
