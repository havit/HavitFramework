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
		private List<Action> afterCommits = null;

		private readonly HashSet<object> insertRegistrations = new HashSet<object>();
		private readonly HashSet<object> updateRegistrations = new HashSet<object>();
		private readonly HashSet<object> deleteRegistrations = new HashSet<object>();

		/// <summary>
		/// DbContext, nad kterým stojí Unit of Work.
		/// </summary>
		protected IDbContext DbContext { get; private set; }

		/// <summary>
		/// SoftDeleteManager používaný v tomto Unit Of Worku.
		/// </summary>
		protected ISoftDeleteManager SoftDeleteManager { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbUnitOfWork(IDbContext dbContext, ISoftDeleteManager softDeleteManager)
		{
			Contract.Requires(dbContext != null);
			Contract.Requires(softDeleteManager != null);

			DbContext = dbContext;
			SoftDeleteManager = softDeleteManager;
		}

		/// <summary>
		/// Zajistí uložení změn registrovaných v Unit of Work.
		/// </summary>
		public void Commit()
		{
			BeforeCommit();
			DbContext.SaveChanges();
			ClearRegistrationHashSets();
			AfterCommit();
		}

		/// <summary>
		/// Asynchronně uloží změny registrované v Unit of Work.
		/// </summary>
		public async Task CommitAsync()
		{
			BeforeCommit();
			await DbContext.SaveChangesAsync();
			ClearRegistrationHashSets();
			AfterCommit();
		}

		/// <summary>
	/// Spuštěno před commitem.
	/// </summary>
		protected internal virtual void BeforeCommit()
		{
			// NOOP
		}

		private void ClearRegistrationHashSets()
		{
			insertRegistrations.Clear();
			updateRegistrations.Clear();
			deleteRegistrations.Clear();
		}

		/// <summary>
		/// Spuštěno po  commitu.
		/// Zajišťuje volání registrovaných after commit akcí (viz RegisterAfterCommitAction).
		/// </summary>
		protected internal virtual void AfterCommit()
		{
			afterCommits?.ForEach(item => item.Invoke());
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
		public void AddForInsert<TEntity>(TEntity entity)
			where TEntity : class
		{
			PerformAddForInsert(entity);
		}

		/// <summary>
		/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
		/// </summary>
		public void AddRangeForInsert<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class
		{
			PerformAddForInsert<TEntity>(entities.ToArray());
		}

		/// <summary>
		/// Zajistí vložení objektu jako změněného (při uložení bude změněn).
		/// </summary>
		public void AddForUpdate<TEntity>(TEntity entity)
			where TEntity : class
		{
			PerformAddForUpdate(entity);
		}

		/// <summary>
		/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
		/// </summary>
		public void AddRangeForUpdate<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class
		{
			PerformAddForUpdate<TEntity>(entities.ToArray());
		}

		/// <summary>
		/// Zajistí odstranění objektu (při uložení bude smazán).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddForDelete<TEntity>(TEntity entity)
			where TEntity : class
		{
			PerformAddForDelete(entity);
		}

		/// <summary>
		/// Zajistí odstranění objektů (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem.
		/// </summary>
		public void AddRangeForDelete<TEntity>(IEnumerable<TEntity> entities)
			where TEntity : class
		{
			PerformAddForDelete<TEntity>(entities.ToArray());
		}

		/// <summary>
		/// Zajistí vložení objektů jako nové objekty (při uložení budou vloženy).
		/// </summary>
		protected virtual void PerformAddForInsert<TEntity>(params TEntity[] entities)
			where TEntity : class
		{
			DbContext.Set<TEntity>().AddRange(entities);
			insertRegistrations.UnionWith(entities);
		}

		/// <summary>
		/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
		/// </summary>
		protected virtual void PerformAddForUpdate<TEntity>(params TEntity[] entities)
			where TEntity : class
		{
			var originalAutoDetectChangesEnabled = DbContext.AutoDetectChangesEnabled;
			DbContext.AutoDetectChangesEnabled = false;
			if (originalAutoDetectChangesEnabled)
			{
				DbContext.DetectChanges();
			}

			foreach (var entity in entities)
			{
				if (DbContext.GetEntityState(entity) == EntityState.Detached)
				{					
					DbContext.SetEntityState(entity, EntityState.Modified);
				}
			}
			updateRegistrations.UnionWith(entities);

			DbContext.AutoDetectChangesEnabled = originalAutoDetectChangesEnabled;
		}

		/// <summary>
		/// Zajistí odstranění objektů (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem - bude jim nastaven příznak smazání a bude nad nimi zavoláno PerformAddForUpdate.
		/// </summary>
		protected virtual void PerformAddForDelete<TEntity>(params TEntity[] entities)
			where TEntity : class
		{
			if (SoftDeleteManager.IsSoftDeleteSupported<TEntity>())
			{
				foreach (TEntity entity in entities)
				{
					SoftDeleteManager.SetDeleted<TEntity>(entity);
				}
				PerformAddForUpdate(entities);
			}
			else
			{
				DbContext.Set<TEntity>().RemoveRange(entities);
				deleteRegistrations.UnionWith(entities);
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
	}
}
