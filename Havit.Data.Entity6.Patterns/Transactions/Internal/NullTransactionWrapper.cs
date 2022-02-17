using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Patterns.Transactions.Internal
{
	/// <summary>
	/// Implementace vůbec neřeší spuštění v transakci.
	/// </summary>
	public class NullTransactionWrapper : ITransactionWrapper
	{
		/// <summary>
		/// Spustí předanou akci.
		/// </summary>
		public void ExecuteWithTransaction(Action action)
		{
			action();
		}
	}
}
