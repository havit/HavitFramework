using Havit.Threading;

namespace Havit.Data.Threading;

    /// <summary>
    /// Places an exclusive lock on an application resource and executes critical section as locked. About exclusive SQL lock see https://docs.microsoft.com/en-us/sql/relational-databases/sql-server-transaction-locking-and-row-versioning-guide?view=sql-server-ver15#lock_modes
    /// </summary>
    /// <remarks>
    /// Use: Multiple app instances must execute same action sequentially.
    /// </remarks>
    public interface IDbLockedCriticalSection : ICriticalSection<string>
    {
    }