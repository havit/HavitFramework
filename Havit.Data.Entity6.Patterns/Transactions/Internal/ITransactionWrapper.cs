namespace Havit.Data.Entity.Patterns.Transactions.Internal;

/// <summary>
/// Zajišťuje spuštění předaného kódu v transakci.
/// Viz implementace.
/// </summary>
// TODO EF Core 6: Přesunout pryč z Havit.Data.Patterns (což možná zjednoduší závislosti)
public interface ITransactionWrapper
{
	/// <summary>
	/// Zadaná akce je spuštěna s transakcí.
	/// </summary>
	void ExecuteWithTransaction(Action action);
}
