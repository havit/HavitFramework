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
	/// Pokusí se vrátit z cache entitu daného typu s daným klíčem. Pokud je entita v cache nalezena a vrácena, vrací true. Jinak false.
	/// </summary>
	bool TryGetEntity<TEntity>(object key, out TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Uloží do cache předanou entitu.
	/// </summary>
	void StoreEntity<TEntity>(TEntity entity)
		where TEntity : class;

	/// <summary>
	/// Pokusí se z cache načíst kolekci nebo one-to-one "back-referenci" dané entity. Pokud je kolekce nebo one-to-one vlastnost entity v cache nalezena a vrácena, vrací true. Jinak false. 
	/// </summary>
	bool TryGetNavigation<TEntity, TPropertyItem>(TEntity entityToLoad, string propertyName)
		where TEntity : class
		where TPropertyItem : class;

	/// <summary>
	/// Uloží do cache kolekci předané entity.
	/// </summary>
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
	void StoreAllKeys<TEntity>(object keys)
		where TEntity : class;

	/// <summary>
	/// Přijme notifikaci o změně entit a zajistí jejich invalidaci v cache.
	/// </summary>
	CacheInvalidationOperation PrepareCacheInvalidation(Changes changes);

}
