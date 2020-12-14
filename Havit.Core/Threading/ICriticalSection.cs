using System;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Threading
{
	/// <summary>
	/// Zajišťuje spuštění kódu kritické sekce nejvýše jedním threadem, resp. vylučuje jeho paralelní běh ve více threadech.
	/// </summary>
	/// <remarks>
	/// Zajišťuje použitelnost CriticalSection&lt;TKey&gt; jako služby, což však není vyžadovaný způsob implementace.
	/// </remarks>
	public interface ICriticalSection<TKey>
	{
		/// <summary>
		/// Vykoná danou akci pod zámkem.
		/// </summary>
		/// <param name="lockValue">
		/// Zámek. 
		/// Na rozdíl od obyčejného locku (resp. Monitoru) provede zamčení nad jeho hodnotou nikoliv nad jeho instancí.
		/// Zámkem proto může být cokoliv, co korektně implementuje operátor porovnání (string, business object, ...).		
		/// </param>
		/// <param name="criticalSection">Kód kritické sekce vykonaný pod zámkem.</param>
		void ExecuteAction(TKey lockValue, Action criticalSection);

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
		Task ExecuteActionAsync(TKey lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default);
	}
}