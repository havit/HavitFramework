using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Předpis služby pro provedení seedování dat.
/// </summary>
public interface IDataSeedRunner
{
	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	void SeedData<TDataSeedProfile>(bool forceRun = false)
		where TDataSeedProfile : IDataSeedProfile, new();

	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	Task SeedDataAsync<TDataSeedProfile>(bool forceRun = false, CancellationToken cancellationToken = default)
		where TDataSeedProfile : IDataSeedProfile, new();

	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	void SeedData(Type dataSeedProfileType, bool forceRun = false);

	/// <summary>
	/// Provede seedování dat.
	/// </summary>
	Task SeedDataAsync(Type dataSeedProfileType, bool forceRun = false, CancellationToken cancellationToken = default);
}