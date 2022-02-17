using System;
using System.Data.Common;
using System.Transactions;

namespace Havit.Data.Entity.Patterns.Transactions.Internal
{
	/// <summary>
	/// Zajišťuje spuštění předaného kódu v transakci.
	/// Transakce je zajištěna pomocí TransactionScope.
	/// </summary>
	public class TransactionScopeTransactionWrapper : ITransactionWrapper
	{
		private readonly TransactionScopeOption transactionScopeOption;
		private readonly IsolationLevel isolationLevel;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public TransactionScopeTransactionWrapper() : this(TransactionScopeOption.Required, IsolationLevel.ReadCommitted)
		{
			// NOOP
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public TransactionScopeTransactionWrapper(TransactionScopeOption transactionScopeOption, IsolationLevel isolationLevel)
		{
			this.transactionScopeOption = transactionScopeOption;
			this.isolationLevel = isolationLevel;
		}

		/// <summary>
		/// Spustí předanou akci pod TransactionScope.
		/// </summary>
		public void ExecuteWithTransaction(Action action)
		{
			using (var scope = new TransactionScope(
				transactionScopeOption,
				new TransactionOptions { IsolationLevel = isolationLevel }))
			{
				action();
				scope.Complete();
			}
		}
	}
}
