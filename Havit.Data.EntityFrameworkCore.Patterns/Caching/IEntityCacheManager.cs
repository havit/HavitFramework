using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching;

/// <summary>
/// Zajišťuje 
/// <list>
///		<item>cachování entit pro metody GetObject[s][Async] v repozitářích a referencí v data loaderu,</item>
///		<item>cachování kolekcí a one-to-one "back-referencí" (navigations) v data loaderu,</item>
///		<item>cachování klíčů objektů pro metody GetAll[Async]</item>
/// </list>
/// Metody se volají pro každou entitu. Je na implementaci, aby se rozhodla, zda bude danou entitu cachovat, či nikoliv.
/// </summary>
public interface IEntityCacheManager
{
	/// <summary>
	/// Vrací true, pokud může být entita daného typu cachována.
	/// Použito před vyhledáváním entity v cache - jinými slovy, to, zda má vůbec význam hledat v cache, se dozvíme z této metody.
	/// </summary>
	bool ShouldCacheEntityType<TEntity>();

	/// <summary>
	/// Pokusí se vrátit z cache entitu daného typu s daným klíčem. Pokud je entita v cache nalezena a vrácena, vrací true. Jinak false.
	/// </summary>
	bool TryGetEntity<TEntity>(object key, out TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Uloží do cache předanou entitu.
	/// </summary>
	/// <remarks>
	/// Entita musí být připojena k <see cref="Havit.Data.EntityFrameworkCore.IDbContext"/> (musí být trackována), aby bylo možné získat smysluplné originální hodnoty.
	/// </remarks>
	void StoreEntity<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Vrací true, pokud půže být daná kolekce dané entity cachována.
	/// </summary>
	bool ShouldCacheEntityTypeNavigation<TEntity>(string propertyName);

	/// <summary>
	/// Pokusí se z cache načíst kolekci nebo one-to-one "back-referenci" dané entity. Pokud je kolekce nebo one-to-one vlastnost entity v cache nalezena a vrácena, vrací true. Jinak false. 
	/// </summary>
	/// <remarks>
	/// Podporovány jsou navigace typu OneToMany, ManyToMany, ManyToManyDecomposedToOneToMany a OneToOne (back-reference).
	/// Navigace typu Reference nejsou podporovány a nemají být předávány k načítání z cache.
	/// </remarks>
	bool TryGetNavigation<TEntity, TPropertyItem>(TEntity entityToLoad, string propertyName)
		where TEntity : class
		where TPropertyItem : class;

	/// <summary>
	/// Uloží do cache kolekci předané entity.
	/// </summary>
	/// <remarks>
	/// Podporovány jsou navigace typu OneToMany, ManyToMany, ManyToManyDecomposedToOneToMany a OneToOne (back-reference).
	/// Navigace typu Reference nejsou podporovány a nemají být předávány k ukládání do cache.
	/// </remarks>
	void StoreNavigation<TEntity, TPropertyItem>(TEntity entity, string propertyName)
		where TEntity : class
		where TPropertyItem : class;

	/// <summary>
	/// Pokusí se vrátit z objekt reprezentující klíče všech entity (pro metodu GetAll). Pokud je objekt v cache nalezen a vrácen, vrací true. Jinak false.
	/// </summary>
	bool TryGetAllKeys<TEntity>(out object keys)
		where TEntity : class;

	/// <summary>
	/// Uloží do cache objekt reprezentující klíče všech entity (pro metodu GetAll).	
	/// </summary>
	/// <param name="keysFunc">Funkce vracející klíče. Je zavolána (a memory alokace tedy udělána) pouze, pokud má dojít k uložení klíčů do cache.</param>
	void StoreAllKeys<TEntity>(Func<object> keysFunc)
		where TEntity : class;

	/// <summary>
	/// Přijme notifikaci o změně entit a zajistí jejich invalidaci v cache.
	/// </summary>
	CacheInvalidationOperation PrepareCacheInvalidation(Changes changes);

	/// <summary>
	/// Odstraní všechny entity (a data související s nimi, např. navigace, atp.) z cache.
	/// </summary>
	void InvalidateAll();
}
