using System;
using System.Collections.Generic;
using Havit.Data.Patterns.DataSeeds.Profiles;

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
		public bool ShouldSeedData(IDataSeedProfile profile, List<Type> dataSeedTypes)
		{
			return true;
		}

		/// <summary>
		/// Metoda je zavolána po dokončení seedování dat.
		/// Nic nedělá.
		/// </summary>
		public void SeedDataCompleted(IDataSeedProfile profile, List<Type> dataSeedTypes)
		{
			// NOOP
		}
	}
}