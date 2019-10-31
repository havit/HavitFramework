using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
		/// <param name="lockValue">
		/// Zámek. 
		/// Na rozdíl od obyčejného locku (resp. Monitoru) provede zamčení nad jeho hodnotou nikoliv nad jeho instancí.
		/// Zámkem proto může být cokoliv, co korektně implementuje operátor porovnání (string, business object, ...).		
		/// </param>
		/// <param name="criticalSection">Kód kritické sekce vykonaný pod zámkem.</param>
		public static void ExecuteAction(object lockValue, Action criticalSection)
		{
			CriticalSectionLock criticalSectionLock = GetCriticalSectionLock(lockValue);

			criticalSectionLock.Semaphore.Wait();
			try
			{
				criticalSection();
			}
			finally
			{
				criticalSectionLock.Semaphore.Release();

				ReleaseCriticalSectionLock(lockValue, criticalSectionLock);
			}
		}

		/// <summary>
		/// Vykoná danou akci pod zámkem.
		/// </summary>
		/// <param name="lockValue">
		/// Zámek. 
		/// Na rozdíl od obyčejného locku (resp. Monitoru) provede zamčení nad jeho hodnotou nikoliv nad jeho instancí.
		/// Zámkem proto může být cokoliv, co korektně implementuje operátor porovnání (string, business object, ...).		
		/// </param>
		/// <param name="criticalSection">Kód kritické sekce vykonaný pod zámkem.</param>
		public static async Task ExecuteActionAsync(object lockValue, Func<Task> criticalSection)
		{
			CriticalSectionLock criticalSectionLock = GetCriticalSectionLock(lockValue);

			await criticalSectionLock.Semaphore.WaitAsync();
			try
			{
				await criticalSection();
			}
			finally
			{
				criticalSectionLock.Semaphore.Release();

				ReleaseCriticalSectionLock(lockValue, criticalSectionLock);
			}
		}

		private static CriticalSectionLock GetCriticalSectionLock(object lockValue)
		{
			lock (_staticLock)
			{
				if (CriticalSectionLocks.TryGetValue(lockValue, out CriticalSectionLock criticalSectionLock))
				{
					criticalSectionLock.UsageCounter += 1;
				}
				else
				{
					criticalSectionLock = new CriticalSectionLock(); // výchozí hodnota čítače je 1
					Debug.Assert(criticalSectionLock.UsageCounter == 1);
					CriticalSectionLocks.Add(lockValue, criticalSectionLock);
				}
				return criticalSectionLock;
			}
		}

		private static void ReleaseCriticalSectionLock(object lockValue, CriticalSectionLock criticalSectionLock)
		{
			// Ať už kritická sekce doběhla dobře nebo došlo k výjimce, musíme snížit čítač použití zámku.
			// Opět pracujeme s čítačem, musíme proto použít statický zámek.
			lock (_staticLock)
			{
				criticalSectionLock.UsageCounter -= 1;
				if (criticalSectionLock.UsageCounter == 0)
				{
					// pokud již nikdo zámek nepoužívá, uklidíme jej
					CriticalSectionLocks.Remove(lockValue);
					criticalSectionLock.Semaphore.Dispose();
				}
			}
		}

		/// <summary>
		/// Nese zámek (semafor) s čítačem použití.
		/// </summary>
		internal class CriticalSectionLock
		{
			/// <summary>
			/// Zámek (semafor).
			/// </summary>
			public SemaphoreSlim Semaphore { get; set; } = new SemaphoreSlim(1, 1);

			/// <summary>
			/// Čítač použití. Výhozí hodnota je 1.
			/// </summary>
			public int UsageCounter { get; set; } = 1;
		}
	}
}
