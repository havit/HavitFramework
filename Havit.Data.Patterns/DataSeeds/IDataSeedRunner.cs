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
		void SeedData();
	}
}