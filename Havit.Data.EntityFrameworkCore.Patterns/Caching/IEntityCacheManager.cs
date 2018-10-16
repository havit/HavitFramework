using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching
{
	/// <summary>
	/// Zajišťuje cachování entit pro metody GetObject[s][Async] v repozitářích.
	/// Dále zajišťuje cachování klíčů objektů pro metody GetAll[Async].
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
		/// Přijme notifikaci o změně entity a zajistí její invalidaci v cache.
		/// </summary>
		/// <remarks>
		/// Tato metoda není generická (na rorzdíl od ostatních), kvůli zamýšlenému použití.
		/// Pro pohodlné volání této metody z DbUnitOfWork.Commit, kde máme k dispozici jen pole objektů,
		/// nepoužijeme tedy generikum, abychom nemuseli metodu volat reflexí pro každou entitu.
		/// </remarks>
		void InvalidateEntity(ChangeType changeType, object entity); 

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
	}
}
