using System;
using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds
{
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
	    void SeedData(Type dataSeedProfileType, bool forceRun = false);

	}
}