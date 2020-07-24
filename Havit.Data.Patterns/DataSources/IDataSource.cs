using System.Linq;

namespace Havit.Data.Patterns.DataSources
{
	/// <summary>
	/// Datový zdroj objektů typu TSource jako IQueryable.
	/// </summary>
	public interface IDataSource<TSource>
	{
		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou odfiltrovány (nejsou v datech).
		/// </summary>
		IQueryable<TSource> Data { get; }

		/// <summary>
		/// Vrací data z datového zdroje jako IQueryable.
		/// Pokud zdroj obsahuje záznamy smazané příznakem, jsou součástí dat.
		/// </summary>
		IQueryable<TSource> DataIncludingDeleted { get; }
	}
}
