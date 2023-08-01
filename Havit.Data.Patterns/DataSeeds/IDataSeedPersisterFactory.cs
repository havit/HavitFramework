namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Factory pro IDataSeedPersister.
/// </summary>
public interface IDataSeedPersisterFactory
{
	/// <summary>
	/// Vytváří/vrací IDataSeedPersister.
	/// </summary>
	IDataSeedPersister CreateService();

	/// <summary>
	/// Uvolňuje službu.
	/// </summary>
	void ReleaseService(IDataSeedPersister service);
}
