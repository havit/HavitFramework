using System.Collections.Generic;

namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Repository objektů typu TEntity.
	/// </summary>
	public interface IRepository<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vrací instanci objektu dle Id.
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		TEntity GetObject(int id);

		/// <summary>
		/// Vrací instance objektů dle Id.
		/// </summary>
		List<TEntity> GetObjects(params int[] ids);

		/// <summary>
		/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
		/// </summary>
		List<TEntity> GetAll();
	}
}
