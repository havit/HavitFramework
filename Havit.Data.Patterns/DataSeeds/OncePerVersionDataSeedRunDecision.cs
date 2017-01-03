using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Rozhoduje, že k spuštění seedování dat dojde jen jednou pro každou verzi aplikace.
	/// Verze aplikace je získávána z FileVersion a data posledního zápisu assembly.
	/// </summary>
	public class OncePerVersionDataSeedRunDecision : IDataSeedRunDecision
	{
		private readonly IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public OncePerVersionDataSeedRunDecision(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister)
		{
			this.dataSeedRunDecisionStatePersister = dataSeedRunDecisionStatePersister;
		}

		/// <summary>
		/// Vrací aktuální stav dle assembly.
		/// </summary>
		protected internal string GetState(IEnumerable<Type> dataSeedTypes)
		{
			return String.Join(",", dataSeedTypes.Select(item => item.Assembly).Distinct().OrderBy(item => item.FullName).Select(assembly =>
				String.Join("|", assembly.FullName, System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion, new System.IO.FileInfo(assembly.Location).LastWriteTimeUtc.ToString("O"))));
		}
	
		/// <summary>
		/// Vrací true, pokud persister obsahuje jinou hodnotu než aktuální stav.
		/// </summary>
		public bool ShouldSeedData(IEnumerable<Type> dataSeedTypes)
		{
			return GetState(dataSeedTypes) != dataSeedRunDecisionStatePersister.ReadCurrentState();
		}

		/// <summary>
		/// Nastaví do persisteru aktuální stav.
		/// </summary>
		public void SeedDataCompleted(IEnumerable<Type> dataSeedTypes)
		{
			dataSeedRunDecisionStatePersister.WriteCurrentState(GetState(dataSeedTypes));
		}
	}
}
