using System.Linq;
using System.Threading.Tasks;

namespace Havit.Data.Entity
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
	}
}