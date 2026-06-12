namespace Havit.Data.Patterns.Tests.DataEntries.Infrastructure;

public class SymbolCodebookEntry
{
	public int Id { get; set; }

	public string Symbol { get; set; }

	public DateTime? Deleted { get; set; }

	public enum Entry
	{
		First = 1,
		Second = 2
	}
}
