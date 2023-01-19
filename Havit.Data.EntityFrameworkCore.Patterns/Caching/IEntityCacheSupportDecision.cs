using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Rozhoduje o tom, zda bude dané entity cachována.
	/// </summary>
	public interface IEntityCacheSupportDecision
	{
		/// <summary>
		/// Vrací true, pokud může být entita daného typu cachována.
		/// Použito před vyhledáváním entity v cache - jinými slovy, to, zda má vůbec význam hledat v cache, se dozvíme z této metody.
		/// </summary>
		/// <remarks>
		/// Očekává se, že metody ShouldCacheEntity a ShouldCacheEntityType budou fungovat konzistentně. Pokud ne, můžeme ukládat do cache instance,
		/// které nebudeme v cache hledat, a nebo naopak nebudeme ukládat do cache entity, avšak budeme je v cache hledat.
		/// </remarks>
		bool ShouldCacheEntityType(Type entityType);

		/// <summary>
		/// Vrací true, pokud mý být daný entita uložena do cache. Pokud vrátí false, entita nebude cachována.
		/// Použito před uložením entity v cache. 
		/// </summary>
		/// <remarks>
		/// Očekává se, že metody ShouldCacheEntity a ShouldCacheEntityType budou fungovat konzistentně. Pokud ne, můžeme ukládat do cache instance,
		/// které nebudeme v cache hledat, a nebo naopak nebudeme ukládat do cache entity, avšak budeme je v cache hledat.
		/// </remarks>
		bool ShouldCacheEntity(object entity);

		/// <summary>
		/// Vrací true, pokud půže být daná kolekce dané entity cachována.
		/// Použito před invalidováním závislostí v cache - jinými slovy, to, zda má vůbec význam invalidovat v cache, se dozvíme z této metody.
		/// </summary>
		/// <remarks>
		/// Očekává se, že metody ShouldCacheEntityCollection a ShouldCacheEntityTypeCollection budou fungovat konzistentně. Pokud ne, můžeme ukládat do cache instance,
		/// které nebudeme v cache hledat, a nebo naopak nebudeme ukládat do cache entity, avšak budeme je v cache hledat.
		/// </remarks>
		bool ShouldCacheEntityTypeCollection(Type entityType, string propertyName);

		/// <summary>
		/// Vrací true, pokud půže být daná kolekce dané entity cachována.
		/// </summary>
		/// <remarks>
		/// Očekává se, že metody ShouldCacheEntityCollection a ShouldCacheEntityTypeCollection budou fungovat konzistentně. Pokud ne, můžeme ukládat do cache instance,
		/// které nebudeme v cache hledat, a nebo naopak nebudeme ukládat do cache entity, avšak budeme je v cache hledat.
		/// </remarks>
		bool ShouldCacheEntityCollection(object entity, string propertyName);

		/// <summary>
		/// Vrací true, pokud mohou být klíče všech entita daného typu cachovány.
		/// </summary>
		bool ShouldCacheAllKeys(Type entityType);
	}
}
