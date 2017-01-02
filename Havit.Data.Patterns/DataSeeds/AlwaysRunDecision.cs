namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Rozhoduje, že k spuštění seedování dat má dojít vždy.
	/// </summary>
	public class AlwaysRunDecision : IDataSeedRunDecision
	{
		/// <summary>
		/// Indikuje, zda má dojík se spuštění seedování dat.
		/// Vždy vrací true.
		/// </summary>
		/// <returns>True.</returns>
		public bool ShouldSeedData()
		{
			return true;
		}

		/// <summary>
		/// Metoda je zavolána po dokončení seedování dat.
		/// Nic nedělá.
		/// </summary>
		public void SeedDataCompleted()
		{
			// NOOP
		}
	}
}