using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Operace pro konfiguraci seedování dat.
	/// </summary>
	public interface IDataSeedFor<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Konfigurace seedování dat.
		/// </summary>
		DataSeedConfiguration<TEntity> Configuration { get; }

		/// <summary>
		/// Nastaví způsob párování dat.
		/// </summary>
		IDataSeedForPaired<TEntity> PairBy(params Expression<Func<TEntity, object>>[] pairBy);

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, TReferencedEntity>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class;

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, ICollection<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class;

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, IEnumerable<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class;

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, List<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class;

		/// <summary>
		/// Přidá do konfigurace vlastnosti, které nebudou aktualizovány. Ty tak slouží jen při zakládání nových objektů.
		/// </summary>
		IDataSeedFor<TEntity> ExcludeUpdate(params Expression<Func<TEntity, object>>[] excludeUpdate);

		/// <summary>
		/// Konfiguruje seedování tak, aby nedošlo k aktualizi existujících objektů, jsou pouze zakládány nové objekty.
		/// </summary>
		IDataSeedFor<TEntity> WithoutUpdate();

		/// <summary>
		/// Konfiguruje seedování tak, aby po uložení seedovaných dat došlo k provedení callbacku (zavolání metody), která je parametrem metody.
		/// </summary>
		IDataSeedFor<TEntity> AfterSave(Action<AfterSaveDataArgs<TEntity>> callback);
	}
}
