namespace Havit.Data.Patterns.Tests.DataEntries.Infrastructure;

/// <summary>
/// Testovací entita bez vlastnosti Symbol - nepodporovaný typ pro DataEntrySymbolService (očekává se NotSupportedException).
/// </summary>
public class EntityWithoutSymbol
{
	public int Id { get; set; }
}
