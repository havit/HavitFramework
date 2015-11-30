namespace Havit.Data.Patterns.QueryServices
{
	/// <summary>
	/// Datový zdroj objektů typu TSource podporující mazání příznakem jako IQueryable.
	/// </summary>
	public interface IDataSourceSoftDelete<TSource> : IDataSource<TSource>
	{	
		/// <summary>
		/// Indikuje, zda mají být vráceny i záznamy smazané příznakem.
		/// </summary>
		bool IncludeDeleted { get; set; }
	}
}
