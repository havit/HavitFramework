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
	public abstract class OncePerVersionDataSeedRunDecisionBase : StateDataSeedRunDecisionBase
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected OncePerVersionDataSeedRunDecisionBase(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister) : base(dataSeedRunDecisionStatePersister)
		{
		}

		/// <summary>
		/// Vrací assembly, která určuje verzi aplikace pro účely seedování.
		/// </summary>
		protected abstract Assembly GetAssembly();

		/// <summary>
		/// Vrací aktuální stav dle assembly.
		/// </summary>
		protected internal override string GetCurrentState()
		{
			Assembly assembly = GetAssembly();

			string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
			string writeTime = new System.IO.FileInfo(assembly.Location).LastWriteTimeUtc.ToString("O");			
			return String.Join("|", fileVersion, writeTime);
		}
	}
}
