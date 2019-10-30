using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Threading
{
	/// <summary>
	/// Zajišťuje spuštění kódu kritické sekce nejvýše jedním threadem, resp. vylučuje jeho paralelní běh ve více threadech.
	/// </summary>
	public static class CriticalSection
	{
		internal static Dictionary<object, CriticalSectionLock> CriticalSectionLocks = new Dictionary<object, CriticalSectionLock>();
		private static object _staticLock = new object();

		/// <summary>
		/// Vykoná danou akci pod zámkem.
		/// </summary>
		/// <param name="lock">
		/// Zámek. 
		/// Na rozdíl od obyčejného locku (resp. Monitoru) provede zamčení nad jeho hodnotou nikoliv nad jeho instancí.
		/// Zámkem proto může být cokoliv, co korektně implementuje operátor porovnání (string, business object, ...).		
		/// </param>
		/// <param name="criticalSection">Kód kritické sekce vykonaný pod zámkem.</param>
		public static void ExecuteAction(object @lock, Action criticalSection)
		{
			CriticalSectionLock currentLockEntry;

			// Bráníme se paralelnímu vytvoření LockEntry. To lze řešit přes ConcurrentDictionary i bez statického zámku.
			// Dále se bráníme situaci, kdy bychom napč. zvyšovali čítač z 0 na 1 zámku, který již byl z dictionary vyhozen. ConcurrentDictionary toto takto neumí, statický zámek toto řeší.
			lock (_staticLock)
			{
				if (CriticalSectionLocks.TryGetValue(@lock, out currentLockEntry))
				{
					currentLockEntry.UsageCounter += 1;
				}
				else
				{
					currentLockEntry = new CriticalSectionLock(); // výchozí hodnota čítače je 1
					Debug.Assert(currentLockEntry.UsageCounter == 1);
					CriticalSectionLocks.Add(@lock, currentLockEntry);
				}
			}

			try
			{
				lock (currentLockEntry) // samotná intance nesoucí čítač slouží jako zámek
				{
					criticalSection();
				}
			}
			finally
			{
				// Ať už kritická sekce doběhla dobře nebo došlo k výjimce, musíme snížit čítač použití zámku.
				// Opět pracujeme s čítačem, musíme proto použít statický zámek.
				lock (_staticLock)
				{
					currentLockEntry.UsageCounter -= 1;
					if (currentLockEntry.UsageCounter == 0)
					{
						// pokud již nikdo zámek nepoužívá, uklidíme jej
						CriticalSectionLocks.Remove(@lock);
					}
				}
			}
		}

		/// <summary>
		/// Reprezentuje skutečný zámek s čítačem použití.
		/// </summary>
		internal class CriticalSectionLock
		{
			/// <summary>
			/// Čítač použití. Výhozí hodnota je 1.
			/// </summary>
			public int UsageCounter { get; set; } = 1;
		}
	}
}
