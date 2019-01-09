namespace Havit.Data.Patterns.Repositories
{
	/// <summary>
	/// Factory pro získávání a vracení repository objektů typu TEtity.
	/// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// Repositories jsou registrovány scoped, proto se této factory popsaná issue týká. Je třeba na každém místě, kde je factory použita, ověřit dopady.
	/// </remarks>
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
