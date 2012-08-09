using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Havit.Business
{
    /// <summary>
    /// Argumenty nesoucí transakci.
    /// </summary>
    public class TransactionEventArgs: EventArgs
    {
        #region Transaction
        /// <summary>
        /// Transakce.
        /// </summary>
        public DbTransaction Transaction
        {
            get { return _transaction; }
            set { _transaction = value; }
        }
        private DbTransaction _transaction; 
        #endregion

        #region Constructors
        public TransactionEventArgs(DbTransaction transaction)
        {
            this._transaction = transaction;
        } 
        #endregion
    }
}
