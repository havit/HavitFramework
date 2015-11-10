using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business
{
	/// <summary>
	/// Argumenty události po uložení objektu.
	/// </summary>
	public class AfterSaveEventArgs : DbTransactionEventArgs
	{
		#region WasNew
		/// <summary>
		/// Indikuje, zda byl objekt před uložením nový.
		/// </summary>
		public bool WasNew
		{
			get { return _wasNew; }
		}
		private readonly bool _wasNew; 
		#endregion
	
		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AfterSaveEventArgs(DbTransaction transaction, bool wasNew)
			: base(transaction)
		{
			_wasNew = wasNew;
		} 
		#endregion
	}
}
