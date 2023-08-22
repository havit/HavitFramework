namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Předpis seedování dat vč. persistence.
/// </summary>
public interface IDataSeed
{
	/// <summary>
	/// Vrátí profil, do kterého daný předpis seedování patří.
	/// </summary>
	Type ProfileType { get; }

	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	void SeedData(IDataSeedPersister dataSeedPersister);

	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	Task SeedDataAsync(IDataSeedPersister dataSeedPersister, CancellationToken cancellationToken = default);

	/// <summary>
	/// Vrací seznam (typů) DataSeedů, na kterých je seedování závislé, tj. vrací seznam dataseedů, které musejí být zpracovány před tímto data seedem.
	/// </summary>
	IEnumerable<Type> GetPrerequisiteDataSeeds();
}