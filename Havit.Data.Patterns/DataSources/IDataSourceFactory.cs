namespace Havit.Data.Patterns.DataSources
{
	/// <summary>
	/// Factory pro získávání a vracení datových zdrojů DataSources&lt;TEntity&gt;.
	/// </summary>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DataSources jsou registrovány jako transientní, proto se této factory popsaná issue netýká.
	/// </remarks>
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
