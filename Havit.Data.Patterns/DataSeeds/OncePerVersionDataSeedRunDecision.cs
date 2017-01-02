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
	public class OncePerVersionDataSeedRunDecision : StateDataSeedRunDecisionBase
	{
		private readonly Assembly dataSeedAssembly;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public OncePerVersionDataSeedRunDecision(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister, Assembly dataSeedAssembly) : base(dataSeedRunDecisionStatePersister)
		{
			this.dataSeedAssembly = dataSeedAssembly;
		}

		/// <summary>
		/// Vrací aktuální stav dle assembly.
		/// </summary>
		protected internal override string GetCurrentState()
		{
			string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(dataSeedAssembly.Location).FileVersion;
			string writeTime = new System.IO.FileInfo(dataSeedAssembly.Location).LastWriteTimeUtc.ToString("O");			
			return String.Join("|", fileVersion, writeTime);
		}
	}
}
