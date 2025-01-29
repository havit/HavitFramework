namespace Havit.Data.EntityFrameworkCore.Threading;

/// <summary>
/// Places an exclusive lock on an application resource and executes critical section as locked. About exclusive SQL lock see https://docs.microsoft.com/en-us/sql/relational-databases/sql-server-transaction-locking-and-row-versioning-guide?view=sql-server-ver15#lock_modes
/// </summary>
/// <remarks>
/// Use: Multiple app instances must execute same action sequentially.
/// </remarks>
public interface IDbLockedCriticalSection
{
	/// <summary>
	/// Enters the critical section and returns a disposable object that releases the lock when disposed.
	/// </summary>
	/// <param name="lockValue">The value to lock on.</param>
	/// <returns>A disposable object that releases the lock when disposed.</returns>	
	IDisposable EnterScope(string lockValue);

	/// <summary>
	/// Enters the critical section asynchronously and returns a disposable object that releases the lock when disposed.
	/// </summary>
	/// <param name="lockValue">The value to lock on.</param>
	/// <param name="cancellationToken">A cancellation token to observe while waiting for the lock.</param>
	/// <returns>A disposable object that releases the lock when disposed.</returns>
	Task<IAsyncDisposable> EnterScopeAsync(string lockValue, CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes the given action under the lock.
	/// </summary>
	/// <param name="lockValue">
	/// Lock. 
	/// Unlike a regular lock (or Monitor), it locks over its value, not its instance.
	/// Therefore, the lock can be anything that correctly implements the comparison operator (string, business object, ...).
	/// </param>
	/// <param name="criticalSection">The code of the critical section executed under the lock.</param>
	void ExecuteAction(string lockValue, Action criticalSection);

	/// <summary>
	/// Executes the given action under the lock.
	/// </summary>
	/// <param name="lockValue">
	/// Lock. 
	/// Unlike a regular lock (or Monitor), it locks over its value, not its instance.
	/// Therefore, the lock can be anything that correctly implements the comparison operator (string, business object, ...).
	/// </param>
	/// <param name="criticalSection">The code of the critical section executed under the lock.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task ExecuteActionAsync(string lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default);
}