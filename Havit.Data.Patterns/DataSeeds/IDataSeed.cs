using System;
using System.Collections.Generic;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Předpis seedování dat vč. persistence.
	/// </summary>
	public interface IDataSeed
	{
		/// <summary>
		/// Provede seedování dat.
		/// </summary>
		void SeedData(IDataSeedPersister dataSeedPersister);

		/// <summary>
		/// Vrací seznam (typů) DataSeedů, na kterých je seedování závislé, tj. vrací seznam dataseedů, které musejí být zpracovány před tímto data seedem.
		/// </summary>
		IEnumerable<Type> GetPrerequisiteDataSeeds();
	}
}