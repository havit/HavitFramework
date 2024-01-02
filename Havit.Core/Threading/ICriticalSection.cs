using System;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Threading;

/// <summary>
/// Ensures that the code of the critical section is executed by at most one thread, or excludes its parallel execution in multiple threads.
/// </summary>
/// <remarks>
/// Ensures the usability of CriticalSection&lt;TKey&gt; as a service, but this is not a required implementation method.
/// </remarks>
public interface ICriticalSection<TKey>
{
	/// <summary>
	/// Executes the given action under the lock.
	/// </summary>
	/// <param name="lockValue">
	/// Lock. 
	/// Unlike a regular lock (or Monitor), it locks over its value, not its instance.
	/// Therefore, the lock can be anything that correctly implements the comparison operator (string, business object, ...).
	/// </param>
	/// <param name="criticalSection">The code of the critical section executed under the lock.</param>
	void ExecuteAction(TKey lockValue, Action criticalSection);

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
	Task ExecuteActionAsync(TKey lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default);
}
