namespace Havit.Data.EntityFrameworkCore.Patterns.QueryServices;

/// <summary>
/// DataFragment - spojení načtených dat při omezení počtu na stránkování/segmentování s počtem celkových záznamů bez ohledu na stránkování/segmentování.
/// </summary>
public class DataFragment<TItem>
{
	/// <summary>
	/// Načtená data dané stránky/segmentu.
	/// </summary>
	public List<TItem> Data { get; init; }

	/// <summary>
	/// Celkový počet záznamů bez ohledu na stránkování/segmentování.
	/// </summary>
	public int TotalCount { get; init; }

	/// <summary>
	/// Deconstructor.
	/// </summary>
	public void Deconstruct(out List<TItem> data, out int totalCount)
	{
		data = this.Data;
		totalCount = this.TotalCount;
	}
}