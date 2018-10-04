namespace Havit.Data.Patterns.DataSources
{
	/// <summary>
	/// Factory pro získávání a vracení datových zdrojů DataSources&lt;TEntity&gt;.
	/// </summary>
	public interface IDataSourceFactory<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Vrací factory.
		/// </summary>
		IDataSource<TEntity> Create();

		/// <summary>
		/// Uvolňuje factory.
		/// </summary>
		void Release(IDataSource<TEntity> dataSource);
	}
}
