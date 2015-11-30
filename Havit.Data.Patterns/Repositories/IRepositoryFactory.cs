namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Factory pro získávání a vracení repository objektů typu TEtity.
	/// </summary>
    public interface IRepositoryFactory<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vrací factory.
		/// </summary>
		IRepository<TEntity> Create();

		/// <summary>
		/// Uvolňuje factory.
		/// </summary>
		void Release(IRepository<TEntity> repository);
	}
}
