using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Patterns.Transactions.Internal
{
	/// <summary>
	/// Zajišťuje spuštění předaného kódu v transakci.
	/// Viz implementace.
	/// </summary>
	public interface ITransactionWrapper
	{
		/// <summary>
		/// Zadaná akce je spuštěna s transakcí.
		/// </summary>
		void ExecuteWithTransaction(Action action);
	}
}
