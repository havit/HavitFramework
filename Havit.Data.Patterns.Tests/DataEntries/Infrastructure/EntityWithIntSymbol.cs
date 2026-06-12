namespace Havit.Data.Patterns.Tests.DataEntries.Infrastructure;

/// <summary>
/// Testovací entita s vlastností Symbol nesprávného typu (int místo string) - nepodporovaný typ pro DataEntrySymbolService (očekává se NotSupportedException).
/// </summary>
public class EntityWithIntSymbol
{
	public int Id { get; set; }

	public int Symbol { get; set; }
}
