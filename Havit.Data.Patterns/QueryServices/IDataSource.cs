using System.Linq;

namespace Havit.Data.Patterns.QueryServices
{
	/// <summary>
	/// Datový zdroj objektů typu TSource jako IQueryable.
	/// </summary>
	public interface IDataSource<TSource>
	{
		/// <summary>
		/// Vrací data z datového zdroje.
		/// </summary>
		IQueryable<TSource> Data { get; }
	}
}
