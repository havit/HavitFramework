namespace Havit.Data.Patterns.Repositories;

/// <summary>
/// Repository objektů typu TEntity.
/// </summary>
public interface IRepository<TEntity, TKey>
	where TEntity : class
{
	/// <summary>
	/// Vrací instanci objektu dle Id.
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
	TEntity GetObject(TKey id);

	/// <summary>
	/// Vrací instanci objektu dle Id.
	/// </summary>
	/// <exception cref="Havit.Data.Patterns.Exceptions.ObjectNotFoundException">Objekt s daným Id nebyl nalezen.</exception>
	Task<TEntity> GetObjectAsync(TKey id, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vrací instance objektů dle Id.
	/// </summary>
	List<TEntity> GetObjects(params TKey[] ids);

	/// <summary>
	/// Vrací instance objektů dle Id.
	/// </summary>
	Task<List<TEntity>> GetObjectsAsync(TKey[] ids, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
	/// </summary>
	List<TEntity> GetAll();

	/// <summary>
	/// Vrací seznam všech (příznakem nesmazaných) objektů typu TEntity.
	/// </summary>
	Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
