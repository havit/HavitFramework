using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Havit.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Threading.Internal
{
    /// <summary>
    /// Kritická sekce zamčená databázovým zámkem.
    /// </summary>
    /// <remarks>
    /// Duplikuje Havit.Data.Threading.DbLockedCriticalSection, kde je použit System.Data.SqlClient, avšak zde používáme Microsoft.Data.SqlClient.
    /// </remarks>
    public class DbLockedCriticalSection : ICriticalSection<string>
    {
        private readonly SqlConnection sqlConnection;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public DbLockedCriticalSection(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        /// <inheritdoc />
        public void ExecuteAction(string lockValue, Action criticalSection)
        {
            bool mustClose = false;
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                sqlConnection.Open();
                mustClose = true;
            }

            try
            {
                GetLock(lockValue, sqlConnection);
                try
                {
                    criticalSection();
                }
                finally
                {
                    ReleaseLock(lockValue, sqlConnection);
                }
            }
            finally
            {
                if (mustClose)
                {
                    sqlConnection.Close();
                }
            }
        }

        /// <inheritdoc />
        public async Task ExecuteActionAsync(string lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default)
        {
            bool mustClose = false;
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                mustClose = true;
            }

            try
            {
                await GetLockAsync(lockValue, sqlConnection, cancellationToken).ConfigureAwait(false);
                try
                {
                    await criticalSection().ConfigureAwait(false);
                }
                finally
                {
                    await ReleaseLockAsync(lockValue, sqlConnection).ConfigureAwait(false); // NO CANCELLATION TOKEN (uvolnit zámek chceme bez ohledu na cancellation token)
                }
            }
            finally
            {
                if (mustClose)
                {
                    await sqlConnection.CloseAsync().ConfigureAwait(false);
                }
            }
        }

        private void GetLock(string lockValue, DbConnection sqlConnection)
        {
            DbCommand sqlCommand = GetLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter);
            sqlCommand.ExecuteNonQuery();
            GetLock_VerifyResultCode((SpGetAppLockResultCode)(int)resultCodeSqlParameter.Value, lockValue);
        }

        private async Task GetLockAsync(string lockValue, DbConnection sqlConnection, CancellationToken cancellationToken)
        {
            DbCommand sqlCommand = GetLock_PrepareCommand(lockValue, sqlConnection, out DbParameter resultCodeSqlParameter);
            await sqlCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            GetLock_VerifyResultCode((SpGetAppLockResultCode)(int)resultCodeSqlParameter.Value, lockValue);
        }

        private DbCommand GetLock_PrepareCommand(string lockValue, DbConnection sqlConnection, out DbParameter resultCodeSqlParameter)
        {
            DbCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.CommandText = "EXEC @ResultCode = sp_getapplock @Resource, 'Exclusive', 'Session', -1";

            DbParameter resourceParameter = sqlCommand.CreateParameter();
            resourceParameter.ParameterName = "@Resource";
            resourceParameter.DbType = System.Data.DbType.String;
            resourceParameter.Value = lockValue;

            resultCodeSqlParameter = sqlCommand.CreateParameter();
            resultCodeSqlParameter.ParameterName = "@ResultCode";
            resultCodeSqlParameter.DbType = System.Data.DbType.Int32;
            resultCodeSqlParameter.Direction = System.Data.ParameterDirection.Output;

            sqlCommand.Parameters.Add(resourceParameter);
            sqlCommand.Parameters.Add(resultCodeSqlParameter);

            sqlCommand.CommandTimeout = 10 * 60 * 1000; // 10 minut
            return sqlCommand;
        }

        private void GetLock_VerifyResultCode(SpGetAppLockResultCode getAppLockResultCode, string lockValue)
        {
            switch (getAppLockResultCode)
            {
                case SpGetAppLockResultCode.Locked:
                case SpGetAppLockResultCode.LockedAfterWaiting:
                    return;
                case SpGetAppLockResultCode.Timeout:
                case SpGetAppLockResultCode.Cancelled:
                case SpGetAppLockResultCode.DeadlockVictim:
                case SpGetAppLockResultCode.Error:
                    throw new DbLockedCriticalSectionException($"Unable to get lock for resource '{lockValue}'. Result code: '{Enum.GetName(typeof(SpGetAppLockResultCode), getAppLockResultCode)}'");
                default:
                    throw new ApplicationException($"Unknown SpGetAppLockResultCode: {Enum.GetName(typeof(SpGetAppLockResultCode), getAppLockResultCode)}");
            }
        }

        private void ReleaseLock(string lockValue, DbConnection sqlConnection)
        {
            using (DbCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        private async Task ReleaseLockAsync(string lockValue, DbConnection sqlConnection)
        {
            using (DbCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection))
            {
                await command.ExecuteNonQueryAsync(CancellationToken.None).ConfigureAwait(false);
            }
        }

        private DbCommand ReleaseLock_PrepareCommand(string lockValue, DbConnection sqlConnection)
        {
            DbCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.CommandText = "EXEC sp_releaseapplock @Resource, 'Session'";

            DbParameter resourceParameter = sqlCommand.CreateParameter();
            resourceParameter.ParameterName = "@Resource";
            resourceParameter.Value = lockValue;

            sqlCommand.Parameters.Add(resourceParameter);

            sqlCommand.CommandTimeout = 10 * 60 * 1000; // 10 minut
            return sqlCommand;
        }

        /// <summary>
        /// Result (code) of calling sp_getapplock.
        /// </summary>
        public enum SpGetAppLockResultCode
        {
            /// <summary>
            /// The lock was successfully granted synchronously.
            /// </summary>
            Locked = 0,

            /// <summary>
            /// The lock was granted successfully after waiting for other incompatible locks to be released.
            /// </summary>
            LockedAfterWaiting = 1,

            /// <summary>
            /// The lock request timed out.
            /// </summary>
            Timeout = -1,

            /// <summary>
            /// The lock request was canceled.
            /// </summary>
            Cancelled = -2,

            /// <summary>
            /// The lock request was chosen as a deadlock victim.
            /// </summary>
            DeadlockVictim = -3,

            /// <summary>
            /// Indicates a parameter validation or other call error.
            /// </summary>
            Error = -999
        }

        /// <summary>
        /// Result (code) of calling sp_releaseapplock.
        /// </summary>
        public enum SpReleaseAppLockResultCode
        {
            /// <summary>
            /// Lock was successfully released.
            /// </summary>
            Released = 0,

            /// <summary>
            /// Indicates a parameter validation or other call error.
            /// </summary>
            Error = -999
        }
    }
}