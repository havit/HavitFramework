using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.Entity.Internal
{
	/// <summary>
	/// Poskytuje služby DbSetu pod interfacem IDbSet&lt;TEntity&gt;.
	/// </summary>
	internal class DbSetInternal<TEntity> : IDbSet<TEntity>
		where TEntity : class
	{
		private readonly DbContext dbContext;
		private readonly DbSet<TEntity> dbSet;
		private readonly Lazy<IKey> primaryKeyLazy;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbSetInternal(DbContext dbContext)
		{
			this.dbContext = dbContext;
			this.dbSet = dbContext.Set<TEntity>(); // zde můžeme bez Lazy - veškerá použití třídy jsou schovaná za lazy
			this.primaryKeyLazy = new Lazy<IKey>(() => dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey());
		}

		/// <summary>
		/// Vrátí data DbSetu jako IQueryable&lt;TEntity&gt;.
		/// DbSet EntityFrameworku je IQueryable&lt;TEntity&gt; sám o sobě. Pro možnost snadné implementace a mockování získáme IQueryable&lt;TEntity&gt; touto metodou.
		/// </summary>
		public IQueryable<TEntity> AsQueryable()
		{
			return dbSet;
		}

		/// <summary>
		/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
		/// </summary>
		public TEntity FindTracked(params object[] keyValues)
		{
			return (TEntity)dbContext.ChangeTracker.GetInfrastructure().TryGetEntry(primaryKeyLazy.Value, keyValues)?.Entity;
		}

		/// <summary>
		/// Finds an entity with the given primary key values. If an entity with the given primary key values exists in the context, then it is returned immediately without making a request to the store. Otherwise, a request is made to the store for an entity with the given primary key values and this entity, if found, is attached to the context and returned. If no entity is found in the context or the store, then null is returned.
		/// </summary>
		public TEntity Find(params object[] keyValues)
		{
			return dbSet.Find(keyValues);
		}

		/// <summary>
		/// Asynchronously finds an entity with the given primary key values. If an entity with the given primary key values exists in the context, then it is returned immediately without making a request to the store. Otherwise, a request is made to the store for an entity with the given primary key values and this entity, if found, is attached to the context and returned. If no entity is found in the context or the store, then null is returned.
		/// </summary>
		public Task<TEntity> FindAsync(params object[] keyValues)
		{
			return dbSet.FindAsync(keyValues);
		}

		/// <summary>
		/// Begins tracking the given entities, and any other reachable entities that are
		/// not already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// </summary>
		public void AddRange(TEntity[] entities)
		{
			dbSet.AddRange(entities);
		}

		/// <summary>
		/// Begins tracking the given entities in the Microsoft.EntityFrameworkCore.EntityState.Modified
		/// state such that they will be updated in the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// All properties of each entity will be marked as modified. To mark only some properties
		/// as modified, use Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0) to begin tracking
		/// each entity in the Microsoft.EntityFrameworkCore.EntityState.Unchanged state
		/// and then use the returned Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry
		/// to mark the desired properties as modified.
		/// A recursive search of the navigation properties will be performed to find reachable
		/// entities that are not already being tracked by the context. These entities will
		/// also begin to be tracked by the context. If a reachable entity has its primary
		/// key value set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Modified
		/// state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state. An entity is considered to have its primary key value set if the primary
		/// key property is set to anything other than the CLR default for the property type.
		/// </summary>
		public void UpdateRange(TEntity[] entities)
		{
			dbSet.UpdateRange(entities);
		}

		/// <summary>
		/// Begins tracking the given entities in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		///  state such that they will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		///  is called.
		/// </summary>
		public void RemoveRange(TEntity[] entities)
		{
			dbSet.RemoveRange(entities);
		}
	}
}