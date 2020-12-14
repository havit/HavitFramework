using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.Threading
{
    /// <summary>
    /// Places an exclusive lock on an application resource and executes critical section as locked. About exclusive SQL lock see https://docs.microsoft.com/en-us/sql/relational-databases/sql-server-transaction-locking-and-row-versioning-guide?view=sql-server-ver15#lock_modes
    /// </summary>
    /// <remarks>
    /// Use: Multiple app instances must execute same action sequentially.
    /// </remarks>
    public class DbLockedCriticalSection : IDbLockedCriticalSection
    {
        private readonly DbLockedCriticalSectionOptions options;

        internal SpGetAppLockResultCode GetAppLockResultCode { get; private set; }
        internal SpReleaseAppLockResultCode ReleaseAppLockResultCode { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public DbLockedCriticalSection(DbLockedCriticalSectionOptions options)
        {
            Contract.Requires<ArgumentNullException>(options != null, nameof(options));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(options.ConnectionString), nameof(options.ConnectionString));

            this.options = options;
        }

        /// <inheritdoc />
        public void ExecuteAction(string lockValue, Action criticalSection)
        {
            using (SqlConnection sqlConnection = new SqlConnection(options.ConnectionString))
            {
                sqlConnection.Open();
                GetLock(lockValue, sqlConnection);

                // try catch logiku neřešíme, uzavřením spojení se uvolní zámek
                switch (GetAppLockResultCode)
                {
                    case SpGetAppLockResultCode.Locked:
                    case SpGetAppLockResultCode.LockedAfterWaiting:
                        criticalSection();
                        break;
                    case SpGetAppLockResultCode.Timeout:
                    case SpGetAppLockResultCode.Cancelled:
                    case SpGetAppLockResultCode.DeadlockVictim:
                    case SpGetAppLockResultCode.Error:
                        throw new DbLockedCriticalSectionException($"Unable to get lock for resource '{lockValue}'. Result code: '{Enum.GetName(typeof(SpGetAppLockResultCode), GetAppLockResultCode)}'");
                    default:
                        throw new ApplicationException($"Unknown SpGetAppLockResultCode: {Enum.GetName(typeof(SpGetAppLockResultCode), GetAppLockResultCode)}");
                }

                ReleaseLock(lockValue, sqlConnection);
                sqlConnection.Close();
            }
        }

        /// <inheritdoc />
        public async Task ExecuteActionAsync(string lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlConnection = new SqlConnection(options.ConnectionString))
            {
                await sqlConnection.OpenAsync(cancellationToken).ConfigureAwait(false);
                await GetLockAsync(lockValue, sqlConnection, cancellationToken).ConfigureAwait(false);

                // try catch logiku neřešíme, uzavřením spojení se uvolní zámek
                switch (GetAppLockResultCode)
                {
                    case SpGetAppLockResultCode.Locked:
                    case SpGetAppLockResultCode.LockedAfterWaiting:
                        // Critical section must be executed without calling ConfigureAwait(false) - we don't know its use and implementation.
#pragma warning disable CAC001 // ConfigureAwaitChecker
                        await criticalSection();
#pragma warning restore CAC001 // ConfigureAwaitChecker
                        break;
                    case SpGetAppLockResultCode.Timeout:
                    case SpGetAppLockResultCode.Cancelled:
                    case SpGetAppLockResultCode.DeadlockVictim:
                    case SpGetAppLockResultCode.Error:
                        throw new DbLockedCriticalSectionException($"Unable to get lock for resource '{lockValue}'. Result code: '{Enum.GetName(typeof(SpGetAppLockResultCode), GetAppLockResultCode)}'");
                    default:
                        throw new ApplicationException($"Unknown SpGetAppLockResultCode: {Enum.GetName(typeof(SpGetAppLockResultCode), GetAppLockResultCode)}");
                }

                // cancellationToken - můžeme si dovolit nečekat na uvolnění zámku - zámek je uvolněn zavřením spojení
                await ReleaseLockAsync(lockValue, sqlConnection, cancellationToken).ConfigureAwait(false); // no cancellation token
                sqlConnection.Close();
            }
        }

        private void GetLock(string lockValue, SqlConnection sqlConnection)
        {
            using (var command = GetLock_PrepareCommand(lockValue, sqlConnection, out SqlParameter resultCodeSqlParameter))
            {
                command.ExecuteNonQuery();
                GetAppLockResultCode = (SpGetAppLockResultCode)(int)resultCodeSqlParameter.Value;
            }
        }

        private async Task GetLockAsync(string lockValue, SqlConnection sqlConnection, CancellationToken cancellationToken)
        {
            using (var command = GetLock_PrepareCommand(lockValue, sqlConnection, out SqlParameter resultCodeSqlParameter))
            {
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false); // no cancellation token
                GetAppLockResultCode = (SpGetAppLockResultCode)(int)resultCodeSqlParameter.Value;
            }
        }

        private SqlCommand GetLock_PrepareCommand(string lockValue, SqlConnection sqlConnection, out SqlParameter resultCodeSqlParameter)
        {
            SqlParameter lockedResourceSqlParameter = new SqlParameter("@Resource", lockValue);
            SqlParameter lockModeSqlParameter = new SqlParameter("@LockMode", "Exclusive"); // Exclusive - Used for data-modification operations, such as INSERT, UPDATE, or DELETE. Ensures that multiple updates cannot be made to the same resource at the same time.
            SqlParameter lockOwnerSqlParameter = new SqlParameter("@LockOwner", "Session");
            SqlParameter lockTimeoutSqlParameter = new SqlParameter("@LockTimeout", options.LockTimeoutMs);            
            resultCodeSqlParameter = new SqlParameter("@ResultCode", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "EXEC @ResultCode = sp_getapplock @Resource, @LockMode, @LockOwner, @LockTimeout";
            command.Parameters.Add(lockedResourceSqlParameter);
            command.Parameters.Add(lockModeSqlParameter);
            command.Parameters.Add(lockOwnerSqlParameter);
            command.Parameters.Add(lockTimeoutSqlParameter);
            command.Parameters.Add(resultCodeSqlParameter);
            command.CommandTimeout = options.SqlCommandTimeoutSeconds;

            return command;
        }

        private void ReleaseLock(string lockValue, SqlConnection sqlConnection)
        {
            using (SqlCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection, out SqlParameter resultCodeSqlParameter))
            {
                command.ExecuteNonQuery();
                ReleaseAppLockResultCode = (SpReleaseAppLockResultCode)(int)resultCodeSqlParameter.Value;
            }
        }

        private async Task ReleaseLockAsync(string lockValue, SqlConnection sqlConnection, CancellationToken cancellationToken)
        {
            using (SqlCommand command = ReleaseLock_PrepareCommand(lockValue, sqlConnection, out SqlParameter resultCodeSqlParameter))
            { 
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false); // no cancellation token
                ReleaseAppLockResultCode = (SpReleaseAppLockResultCode)(int)resultCodeSqlParameter.Value;
            }
        }

        private SqlCommand ReleaseLock_PrepareCommand(string lockValue, SqlConnection sqlConnection, out SqlParameter resultCodeSqlParameter)
        {
            SqlParameter lockedResourceSqlParameter = new SqlParameter("Resource", lockValue);
            SqlParameter lockOwnerSqlParameter = new SqlParameter("LockOwner", "Session");
            resultCodeSqlParameter = new SqlParameter("@ResultCode", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "EXEC @ResultCode = sp_releaseapplock @Resource, @LockOwner";
            command.Parameters.Add(lockedResourceSqlParameter);
            command.Parameters.Add(lockOwnerSqlParameter);
            command.Parameters.Add(resultCodeSqlParameter);
            command.CommandTimeout = options.SqlCommandTimeoutSeconds;

            return command;
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
