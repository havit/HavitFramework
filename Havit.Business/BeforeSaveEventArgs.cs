using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business
{
	/// <summary>
	/// Argumenty události před uložením objektu.
	/// </summary>
	public class BeforeSaveEventArgs : DbTransactionEventArgs
	{
		#region Constructors
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public BeforeSaveEventArgs(DbTransaction transaction)
			: base(transaction)
		{

		} 
		#endregion
	}
}
