using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks
{
	/// <summary>
	/// Unit of Work postavená nad <see cref="DbContext" />.
	/// </summary>
	public class DbUnitOfWork : IUnitOfWork, IUnitOfWorkAsync
	{
		private readonly IBeforeCommitProcessorsRunner beforeCommitProcessorsRunner;
		private readonly IEntityValidationRunner entityValidationRunner;
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
		/// EntityCacheManager používaný repository.
		/// </summary>
		protected IEntityCacheManager EntityCacheManager { get; private set; }			

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbUnitOfWork(IDbContext dbContext, ISoftDeleteManager softDeleteManager, IEntityCacheManager entityCacheManager, IBeforeCommitProcessorsRunner beforeCommitProcessorsRunner, IEntityValidationRunner entityValidationRunner)
		{
			Contract.Requires(dbContext != null);
			Contract.Requires(softDeleteManager != null);

			DbContext = dbContext;
			SoftDeleteManager = softDeleteManager;
			EntityCacheManager = entityCacheManager;
			this.beforeCommitProcessorsRunner = beforeCommitProcessorsRunner;
			this.entityValidationRunner = entityValidationRunner;
		}

		/// <summary>
		/// Zajistí uložení změn registrovaných v Unit of Work.
		/// </summary>
		public void Commit()
		{
			BeforeCommit();
			beforeCommitProcessorsRunner.Run(GetAllKnownChanges());

			Changes allKnownChanges = GetAllKnownChanges(); // práme se na změny znovu, runnery mohli seznam objektů k uložení změnit
			entityValidationRunner.Validate(allKnownChanges);
			DbContext.SaveChanges();

			ClearRegistrationHashSets();
			InvalidateEntityCache(allKnownChanges);

			AfterCommit();
		}

		/// <summary>
		/// Asynchronně uloží změny registrované v Unit of Work.
		/// </summary>
		public async Task CommitAsync()
		{
			BeforeCommit();
			beforeCommitProcessorsRunner.Run(GetAllKnownChanges());

			Changes allKnownChanges = GetAllKnownChanges(); // práme se na změny znovu, runnery mohli seznam objektů k uložení změnit
			entityValidationRunner.Validate(allKnownChanges);
			await DbContext.SaveChangesAsync().ConfigureAwait(false);

			ClearRegistrationHashSets();
			InvalidateEntityCache(allKnownChanges);

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
			List<Action> registeredAfterCommitActiond = afterCommits;
			// Neprve vyčistíme afterCommits, pak je teprve spustíme.
			// Tím umožníme rekurzivní volání Commitu (resp. volání Commitu z AfterCommitAction), při opačném pořadí (nejdřív spustit, pak vyčistit) dojde k zacyklení.
			afterCommits = null;
			registeredAfterCommitActiond?.ForEach(item => item.Invoke());
		}

		/// <summary>
		/// Registruje akci k provedení po commitu. Akce je provedena metodou AfterCommit.
		/// Při opakovaném commitu již akce není volána.
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
		/// Vrací všechny známé změny ke commitu, vychází z registrací objektů a používá také DbContext a jeho changetracker.
		/// </summary>
		protected internal Changes GetAllKnownChanges()
		{
			var inserts = DbContext.GetObjectsInState(EntityState.Added, suppressDetectChanges: false).Union(insertRegistrations).ToArray();
			var updates = DbContext.GetObjectsInState(EntityState.Modified, suppressDetectChanges: true /* pokud je zapnutý changetracker (jako že je), byl již spuštěn v rámci předchozího volání */).Union(updateRegistrations).ToArray();
			var deletes = DbContext.GetObjectsInState(EntityState.Deleted, suppressDetectChanges: true /* pokud je zapnutý changetracker (jako že je), byl již spuštěn v rámci předchozího volání */).Union(deleteRegistrations).ToArray();

			return new Changes
			{
				Inserts = inserts,
				Updates = updates,
				Deletes = deletes
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
			if (entities.Length > 0)
			{
				DbContext.Set<TEntity>().AddRange(entities);
				insertRegistrations.UnionWith(entities);
			}
		}

		/// <summary>
		/// Zajistí vložení objektů jako změněné objekty (při uložení budou změněny).
		/// </summary>
		protected virtual void PerformAddForUpdate<TEntity>(params TEntity[] entities)
			where TEntity : class
		{
			if (entities.Length > 0)
			{
				// registrace na DbSetu pro Update označí entity za modifikované a budou uložené celé bez ohledu na to, zda se na nich skutečně něco změnilo
				// proto implementujeme tak, že trackované entity neřešíme (a předpokládáme, že svou práci provede change tracker)
				// netrackované entity zaregistrujeme jako změněné

				// z výkonových důvodů se očekává, že volaná metoda GetEntityState nevolá change tracker

				DbContext.Set<TEntity>().UpdateRange(entities.Where(entity => DbContext.GetEntityState(entity) == EntityState.Detached).ToArray());
				updateRegistrations.UnionWith(entities);
			}
		}

		/// <summary>
		/// Zajistí odstranění objektů (při uložení budou smazány).
		/// Objekty podporující mazání příznakem budou smazány příznakem - bude jim nastaven příznak smazání a bude nad nimi zavoláno PerformAddForUpdate.
		/// </summary>
		protected virtual void PerformAddForDelete<TEntity>(params TEntity[] entities)
			where TEntity : class
		{
			if (entities.Length > 0) 
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
		}

		/// <summary>
		/// Oznámí k invalidaci všechny změněné objekty.
		/// </summary>
		protected virtual void InvalidateEntityCache(Changes allKnownChanges)
		{
			IEntityCacheManager cacheManager = this.EntityCacheManager;
            
            if (allKnownChanges.Inserts.Length > 0)
            {
                foreach (var insertedEntity in allKnownChanges.Inserts)
                {
                    cacheManager.InvalidateEntity(ChangeType.Insert, insertedEntity);
                }
            }

            if (allKnownChanges.Updates.Length > 0)
            {
                foreach (var updatedEntity in allKnownChanges.Updates)
                {
                    cacheManager.InvalidateEntity(ChangeType.Update, updatedEntity);
                }
            }

            if (allKnownChanges.Deletes.Length > 0)
            {
                foreach (var deletedEntity in allKnownChanges.Deletes)
                {
                    cacheManager.InvalidateEntity(ChangeType.Delete, deletedEntity);
                }
            }
		}

	}
}
