using System.Collections.Generic;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Repository objektů typu TEntity s podporou asynchronního zpracování.
	/// </summary>	
	public interface IRepositoryAsync<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vrací instanci objektu dle Id.
		/// </summary>
		/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
		Task<TEntity> GetObjectAsync(int id);

		/// <summary>
		/// Vrací instance objektů dle Id.
		/// </summary>
		Task<List<TEntity>> GetObjectsAsync(params int[] ids);

		/// <summary>
		/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
		/// </summary>
		Task<List<TEntity>> GetAllAsync();
	}
}