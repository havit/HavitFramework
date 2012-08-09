using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business
{
    /// <summary>
    /// Argument nesoucí instanci databázové transakce.
    /// </summary>
    public class DbTransactionEventArgs: EventArgs
    {
        #region Transaction
        /// <summary>
        /// Transakce.
		/// Pro OnBeforeSave a OnAfterSave nemùže být v pøípadì ActiveRecordBusinessObjectBase null, v pøípadì holého BusinessObjectBase ano.
        /// </summary>
        public DbTransaction Transaction
        {
            get { return _transaction; }
        }
        private DbTransaction _transaction; 
        #endregion

        #region Constructors
        public DbTransactionEventArgs(DbTransaction transaction)
        {
            this._transaction = transaction;
        } 
        #endregion
    }
}
