using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Threading;

/// <summary>
/// Zajišťuje spuštění kódu kritické sekce nejvýše jedním threadem, resp. vylučuje jeho paralelní běh ve více threadech.
/// </summary>
/// <remarks>
/// I když třída díky interface usnadňuje možnost použití jako služby, není takové použití vyžadováno. 
/// Bez obav tam, kde potřebujeme, vytvářejme instance bez DI containeru.
/// </remarks>
public class CriticalSection<TKey> : ICriticalSection<TKey>
{
	private readonly Dictionary<TKey, CriticalSectionLock> criticalSectionLocks;
	internal Dictionary<TKey, CriticalSectionLock> CriticalSectionLocks => criticalSectionLocks; // pro unit testy

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CriticalSection()
	{
		criticalSectionLocks = new Dictionary<TKey, CriticalSectionLock>();
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public CriticalSection(IEqualityComparer<TKey> comparer)
	{
		criticalSectionLocks = new Dictionary<TKey, CriticalSectionLock>(comparer);
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
	public void ExecuteAction(TKey lockValue, Action criticalSection)
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
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task ExecuteActionAsync(TKey lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default)
	{
		CriticalSectionLock criticalSectionLock = GetCriticalSectionLock(lockValue);

		await criticalSectionLock.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
#pragma warning disable CAC001 // ConfigureAwaitChecker
			await criticalSection();
#pragma warning restore CAC001 // ConfigureAwaitChecker
		}
		finally
		{
			criticalSectionLock.Semaphore.Release();

			ReleaseCriticalSectionLock(lockValue, criticalSectionLock);
		}
	}

	internal CriticalSectionLock GetCriticalSectionLock(TKey lockValue)
	{
		// Pracujeme s čítačem, musíme proto použít "globální" zámek.
		lock (criticalSectionLocks) // použijeme dictionary pro zámek
		{
			if (criticalSectionLocks.TryGetValue(lockValue, out CriticalSectionLock criticalSectionLock))
			{
				criticalSectionLock.UsageCounter += 1;
			}
			else
			{
				criticalSectionLock = new CriticalSectionLock(); // výchozí hodnota čítače je 1
				Debug.Assert(criticalSectionLock.UsageCounter == 1);
				criticalSectionLocks.Add(lockValue, criticalSectionLock);
			}
			return criticalSectionLock;
		}
	}

	internal void ReleaseCriticalSectionLock(TKey lockValue, CriticalSectionLock criticalSectionLock)
	{
		// Ať už kritická sekce doběhla dobře nebo došlo k výjimce, musíme snížit čítač použití zámku.
		// Opět pracujeme s čítačem, musíme proto použít "globální" zámek.
		lock (criticalSectionLocks) // použijeme dictionary pro zámek
		{
			criticalSectionLock.UsageCounter -= 1;
			if (criticalSectionLock.UsageCounter == 0)
			{
				// pokud již nikdo zámek nepoužívá, uklidíme jej
				criticalSectionLocks.Remove(lockValue);
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
