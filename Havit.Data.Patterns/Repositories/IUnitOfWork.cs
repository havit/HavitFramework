using System;
using System.Collections.Generic;

namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Unit of Work.
	/// </summary>
	public interface IUnitOfWork
	{
		/// <summary>
		/// Uloží změny registrované v Unit of Work.
		/// </summary>
		void Commit();

		/// <summary>
		/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
		/// </summary>
		void AddForInsert<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
		/// </summary>
		void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
		/// </summary>
		void AddForUpdate<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
		/// </summary>
		void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Zajistí odstranění objektu (při uložení bude smazán).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		void AddForDelete<TEntity>(TEntity entity)
			where TEntity : class;

		/// <summary>
		/// Zajistí odstranění objektů (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class;

		/// <summary>
		/// Registruje akci k provedení po commitu. Akce je provedena metodou AfterCommit.
		/// </summary>
		void RegisterAfterCommitAction(Action action);
	}
}
