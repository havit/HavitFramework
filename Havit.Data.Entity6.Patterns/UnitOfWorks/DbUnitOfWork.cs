using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.Helpers;
using Havit.Data.Entity.Patterns.SoftDeletes;
using Havit.Data.Patterns.Repositories;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.UnitOfWorks
{
	/// <summary>
	/// Unit of Work postavená nad <see cref="DbContext" />.
	/// </summary>
	public class DbUnitOfWork : IUnitOfWork, IUnitOfWorkAsync
	{
		private readonly ISoftDeleteManager softDeleteManager;

		private bool commited = false;

		private List<Action> afterCommits = null;

		private readonly HashSet<object> insertRegistrations = new HashSet<object>();
		private readonly HashSet<object> updateRegistrations = new HashSet<object>();
		private readonly HashSet<object> deleteRegistrations = new HashSet<object>();

		/// <summary>
		/// DbContext, nad kterým stojí Unit of Work.
		/// </summary>
		protected IDbContext DbContext { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbUnitOfWork(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		{
			this.softDeleteManager = softDeleteManager;
			Contract.Assert(dbContext != null);
			this.DbContext = dbContext;			
		}

		/// <summary>
		/// Zajistí uložení změn registrovaných v Unit of Work.
		/// </summary>
		public void Commit()
		{
			VerifyNotCommited();

			BeforeCommit();
			DbContext.SaveChanges();
			AfterCommit();

			commited = true;
		}

		/// <summary>
		/// Asynchronně uloží změny registrované v Unit of Work.
		/// </summary>
		public async Task CommitAsync()
		{
			VerifyNotCommited();

			BeforeCommit();
			await DbContext.SaveChangesAsync();
			AfterCommit();

			commited = true;
		}

		/// <summary>
		/// Spuštěno před commitem.
		/// </summary>
		protected internal virtual void BeforeCommit()
		{
			// NOOP
		}

		/// <summary>
		/// Spuštěno po  commitu.
		/// Zajišťuje volání registrovaných after commit akcí (viz RegisterAfterCommitAction).
		/// </summary>
		protected internal virtual void AfterCommit()
		{
			afterCommits?.ForEach(item => item());
		}

		/// <summary>
		/// Registruje akci k provedení po commitu. Akce je provedena metodou AfterCommit.
		/// </summary>
		public void RegisterAfterCommitAction(Action action)
		{
			if (afterCommits == null)
			{
				afterCommits = new List<Action>();
			}
			afterCommits.Add(action);
		}

		/// <summary>
		/// Vrací změny registrované ke commitu, vychází pouze z registrací objektů a nepoužívá DbContext a jeho changetracker.
		/// </summary>
		protected internal Changes GetRegisteredChanges()
		{
			VerifyNotCommited();

			return new Changes
			{
				Inserts = insertRegistrations.ToArray(),
				Updates = updateRegistrations.ToArray(),
				Deletes = deleteRegistrations.ToArray()
			};
		}

		/// <summary>
		/// Vrací všechny známé změny ke commitu, vychází z registrací objektů a používá také DbContext a jeho changetracker.
		/// </summary>
		protected internal Changes GetAllKnownChanges()
		{
			VerifyNotCommited();

			return new Changes
			{
				Inserts = DbContext.GetObjectsInState(EntityState.Added).Union(insertRegistrations).ToArray(),
				Updates = DbContext.GetObjectsInState(EntityState.Modified).Union(updateRegistrations).ToArray(),
				Deletes = DbContext.GetObjectsInState(EntityState.Deleted).Union(deleteRegistrations).ToArray()
			};
		}

		/// <summary>
		/// Zajistí vložení objektu jako nového objektu (při uložení bude vložen).
		/// </summary>
		public void AddForInsert<TEntity>(TEntity item)
			where TEntity : class
		{
			VerifyNotCommited();

			DbContext.Set<TEntity>().Add(item);

			insertRegistrations.Add(item);
		}

		/// <summary>
		/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
		/// </summary>
		public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class
		{
			VerifyNotCommited();

			var itemsList = items.ToList();
			DbContext.Set<TEntity>().AddRange(itemsList);
			insertRegistrations.UnionWith(itemsList);
		}

		/// <summary>
		/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
		/// </summary>
		public void AddForUpdate<TEntity>(TEntity item)
			where TEntity : class
		{
			VerifyNotCommited();

			DbContext.SetEntityState(item, EntityState.Modified);
			updateRegistrations.Add(item);
		}

		/// <summary>
		/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
		/// </summary>
		public void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class
		{
			VerifyNotCommited();

			List<TEntity> itemsList = items.ToList();
			foreach (var item in itemsList)
			{
				DbContext.Set<TEntity>().Attach(item);
			}
			updateRegistrations.UnionWith(itemsList);
		}

		/// <summary>
		/// Zajistí odstranění objektu (při uložení bude smazán).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddForDelete<TEntity>(TEntity item)
			where TEntity : class
		{
			VerifyNotCommited();

			if (softDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				softDeleteManager.SetDeleted(item);
				AddForUpdate(item);
			}
			else
			{
				DbContext.Set<TEntity>().Remove(item);
				deleteRegistrations.Add(item);
			}
		}

		/// <summary>
		/// Zajistí odstranění objektů (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> items)
			where TEntity : class
		{
			VerifyNotCommited();

			var itemsList = items.ToList();
			if (softDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				foreach (TEntity item in itemsList)
				{
					softDeleteManager.SetDeleted<TEntity>(item);
				}
				AddRangeForUpdate(itemsList);
			}
			else
			{
				DbContext.Set<TEntity>().RemoveRange(itemsList);
				deleteRegistrations.UnionWith(itemsList);
			}

		}

		/// <summary>
		/// Vyhazuje výjimku, pokud existuje objekt, který je registrován jako změněný v DbContextu (díky changetrackeru), ale není registrován v tomto UnitOfWorku.
		/// </summary>
		/// <exception cref="InvalidOperationException">Existuje objekt, který je registrován jako změněný v DbContextu (díky changetrackeru), ale není registrován v tomto UnitOfWorku.</exception>
		protected void VerifyAllChangesAreRegistered()
		{
			object[] notRegisteredUpdates = GetNotRegisteredChanges();
			if (notRegisteredUpdates.Any())
			{
				List<Type> types = notRegisteredUpdates.Select(item => item.GetType()).Distinct().ToList();
				string typesMessage = String.Join(", ", types.Select(type => type.ToString()));

				InvalidOperationException exception = new InvalidOperationException($"UnitOfWork does not have registered all changes. Missing are objects of type {typesMessage}.");
				exception.Data.Add("NotRegisteredUpdates", notRegisteredUpdates);
				throw exception;
			}
		}

		/// <summary>
		/// Vrací objekty, které jsou registrovány jako změněné v DbContextu (díky changetrackeru), ale není registrován v tomto UnitOfWorku.
		/// </summary>
		protected object[] GetNotRegisteredChanges()
		{
			return DbContext.GetObjectsInState(EntityState.Added | EntityState.Modified | EntityState.Deleted).Except(insertRegistrations).Except(updateRegistrations).Except(deleteRegistrations).ToArray();
		}

		private void VerifyNotCommited()
		{
			if (commited)
			{
				throw new InvalidOperationException("UnitOfWork has been already commited.");
			}
		}
	}
}
