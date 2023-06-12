using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Internal
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
		private readonly Lazy<IStateManager> stateManagerLazy;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbSetInternal(DbContext dbContext)
		{
			this.dbContext = dbContext;
			this.dbSet = dbContext.Set<TEntity>(); // zde můžeme bez Lazy - veškerá použití třídy jsou schovaná za lazy
			this.primaryKeyLazy = new Lazy<IKey>(() => dbContext.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey(), LazyThreadSafetyMode.None);
#pragma warning disable EF1001 // Internal EF Core API usage.
			this.stateManagerLazy = new Lazy<IStateManager>(() => dbContext.GetService<IStateManager>(), LazyThreadSafetyMode.None);
#pragma warning restore EF1001 // Internal EF Core API usage.
		}

		/// <inheritdoc />
		public IQueryable<TEntity> AsQueryable(string queryTag)
		{
			return !String.IsNullOrEmpty(queryTag)
				? dbSet.TagWith(queryTag)
				: dbSet;
		}

		/// <summary>
		/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
		/// </summary>
		public TEntity FindTracked(params object[] keyValues)
		{
#pragma warning disable EF1001 // Internal EF Core API usage.
			return (TEntity)stateManagerLazy.Value.TryGetEntry(primaryKeyLazy.Value, keyValues)?.Entity;
#pragma warning restore EF1001 // Internal EF Core API usage.
		}

		/// <summary>
		/// Asynchronously finds an entity with the given primary key values. If an entity with the given primary key values exists in the context, then it is returned immediately without making a request to the store. Otherwise, a request is made to the store for an entity with the given primary key values and this entity, if found, is attached to the context and returned. If no entity is found in the context or the store, then null is returned.
		/// </summary>
		public ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default)
		{
			return dbSet.FindAsync(keyValues, cancellationToken);
		}

		/// <summary>
		/// Begins tracking the given entity, and any other reachable entities that are not
		/// already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		/// state of only a single entity.
		/// </summary>
		public void Add(TEntity entity)
		{
			dbSet.Add(entity);
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
		/// Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Modified
		/// state such that it will be updated in the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// All properties of the entity will be marked as modified. To mark only some properties
		/// as modified, use Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0) to begin tracking
		/// the entity in the Microsoft.EntityFrameworkCore.EntityState.Unchanged state and
		/// then use the returned Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry
		/// to mark the desired properties as modified.
		/// A recursive search of the navigation properties will be performed to find reachable
		/// entities that are not already being tracked by the context. These entities will
		/// also begin to be tracked by the context. If a reachable entity has its primary
		/// key value set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Modified
		/// state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state. An entity is considered to have its primary key value set if the primary
		/// key property is set to anything other than the CLR default for the property type.
		/// Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		/// state of only a single entity.
		/// </summary>
		public void Update(TEntity entity)
		{
			dbSet.Update(entity);
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
		/// Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		/// state such that it will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// </summary>
		/// <remarks>
		/// If the entity is already tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state then the context will stop tracking the entity (rather than marking it
		/// as Microsoft.EntityFrameworkCore.EntityState.Deleted) since the entity was previously
		/// added to the context and does not exist in the database.
		/// Any other reachable entities that are not already being tracked will be tracked
		/// in the same way that they would be if Microsoft.EntityFrameworkCore.DbSet`1.Attach(`0)
		/// was called before calling this method. This allows any cascading actions to be
		/// applied when Microsoft.EntityFrameworkCore.DbContext.SaveChanges is called.
		/// Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		/// state of only a single entity.
		/// </remarks>
		public void Remove(TEntity entity)
		{
			dbSet.Remove(entity);
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

		/// <summary>
		/// Begins tracking the given entity in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		/// state such that no operation will be performed when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// A recursive search of the navigation properties will be performed to find reachable
		/// entities that are not already being tracked by the context. These entities will
		/// also begin to be tracked by the context. If a reachable entity has its primary
		/// key value set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Unchanged
		/// state. If the primary key value is not set then it will be tracked in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state. An entity is considered to have its primary key value set if the primary
		/// key property is set to anything other than the CLR default for the property type.
		/// Use Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry.State to set the
		/// state of only a single entity.
		/// </summary>
		public void Attach(TEntity entity)
		{
			dbSet.Attach(entity);
		}

		/// <summary>
		/// Begins tracking the given entities in the Unchanged state such that no operation will be
		/// performed when SaveChanges() is called.
		/// A recursive search of the navigation properties will be performed to find reachable entities
		/// that are not already being tracked by the context.These entities will also begin to be tracked
		/// by the context. If a reachable entity has its primary key value set then it will be tracked
		/// in the Unchanged state.If the primary key value is not set then it will be tracked in the Added state.
		/// An entity is considered to have its primary key value set if the primary key property is set
		/// to anything other than the CLR default for the property type.
		/// </summary>
		public void AttachRange(TEntity[] entities)
		{
			dbSet.AttachRange(entities);
		}

	}
}