using System;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Entry point pro persistenci závislých (child) data seedů.
	/// Interně použito pro ukládání lokalizací lokalizovaných objektů, atp.	
	/// </summary>
	public class ChildDataSeedConfigurationEntry
	{
		/// <summary>
		/// Entry point pro persistenci závislých (child) data seedů.
		/// </summary>
		public Action<IDataSeedPersister> SaveAction { get; private set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public ChildDataSeedConfigurationEntry(Action<IDataSeedPersister> saveAction)
		{
			SaveAction = saveAction;
		}

	}
}
