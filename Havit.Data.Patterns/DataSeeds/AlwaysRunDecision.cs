using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Rozhoduje, že k spuštění seedování dat má dojít vždy.
/// </summary>
public class AlwaysRunDecision : IDataSeedRunDecision
{
	/// <summary>
	/// Indikuje, zda má dojít ke spuštění seedování dat.
	/// Vždy vrací true.
	/// </summary>
	/// <returns>True.</returns>
	public bool ShouldSeedData(IDataSeedProfile profile, List<Type> dataSeedTypes)
	{
		return true;
	}

	/// <summary>
	/// Indikuje, zda má dojít ke spuštění seedování dat.
	/// Vždy vrací true.
	/// </summary>
	/// <returns>True.</returns>
	public Task<bool> ShouldSeedDataAsync(IDataSeedProfile profile, List<Type> dataSeedTypes, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(true);
	}

	/// <summary>
	/// Metoda je zavolána po dokončení seedování dat.
	/// Nic nedělá.
	/// </summary>
	public void SeedDataCompleted(IDataSeedProfile profile, List<Type> dataSeedTypes)
	{
		// NOOP
	}

	/// <summary>
	/// Metoda je zavolána po dokončení seedování dat.
	/// Nic nedělá.
	/// </summary>
	public Task SeedDataCompletedAsync(IDataSeedProfile profile, List<Type> dataSeedTypes, CancellationToken cancellationToken = default)
	{
		// NOOP
		return Task.CompletedTask;
	}
}