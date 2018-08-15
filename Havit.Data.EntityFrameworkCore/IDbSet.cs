using System.Linq;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Abstrakce DbSet a jeho služeb pro možnost snadného mockování v závislostech.
	/// </summary>
	public interface IDbSet<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vrátí data DbSetu jako IQueryable&lt;TEntity&gt;.
		/// DbSet EntityFrameworku je IQueryable&lt;TEntity&gt; sám o sobě. Pro možnost snadné implementace a mockování získáme IQueryable&lt;TEntity&gt; touto metodou.
		/// </summary>
		IQueryable<TEntity> AsQueryable();

		/// <summary>
		/// Vyhledá entitu v načtených (trackovaných objektech). Pokud objekt není nalezen, vrací null.
		/// </summary>
		TEntity FindTracked(params object[] keyValues);

		/// <summary>
		/// Finds an entity with the given primary key values. If an entity with the given primary key values exists in the context, then it is returned immediately without making a request to the store. Otherwise, a request is made to the store for an entity with the given primary key values and this entity, if found, is attached to the context and returned. If no entity is found in the context or the store, then null is returned.
		/// </summary>
		TEntity Find(params object[] keyValues);

		/// <summary>
		/// Asynchronously finds an entity with the given primary key values. If an entity with the given primary key values exists in the context, then it is returned immediately without making a request to the store. Otherwise, a request is made to the store for an entity with the given primary key values and this entity, if found, is attached to the context and returned. If no entity is found in the context or the store, then null is returned.
		/// </summary>
		Task<TEntity> FindAsync(params object[] keyValues);

		/// <summary>
		/// Begins tracking the given entities, and any other reachable entities that are
		/// not already being tracked, in the Microsoft.EntityFrameworkCore.EntityState.Added
		/// state such that they will be inserted into the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		/// is called.
		/// </summary>
		void AddRange(TEntity[] entities);

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
		void UpdateRange(TEntity[] entities);

		/// <summary>
		/// Begins tracking the given entities in the Microsoft.EntityFrameworkCore.EntityState.Deleted
		///  state such that they will be removed from the database when Microsoft.EntityFrameworkCore.DbContext.SaveChanges
		///  is called.
		/// </summary>
		void RemoveRange(TEntity[] entities);
	}
}