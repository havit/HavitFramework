using System.Diagnostics;

namespace Havit.Threading;

/// <summary>
/// Ensures that the code of the critical section is executed by at most one thread, or excludes its parallel execution in multiple threads.
/// </summary>
/// <remarks>
/// Although the class facilitates the possibility of being used as a service thanks to the interface, such use is not required.
/// Without hesitation, where we need to, we create instances without a DI container.
/// </remarks>
public class CriticalSection<TKey>
{
	private readonly Dictionary<TKey, CriticalSectionLock> criticalSectionLocks;
	internal Dictionary<TKey, CriticalSectionLock> CriticalSectionLocks => criticalSectionLocks; // for unit tests

	/// <summary>
	/// Constructor.
	/// </summary>
	public CriticalSection()
	{
		criticalSectionLocks = new Dictionary<TKey, CriticalSectionLock>();
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public CriticalSection(IEqualityComparer<TKey> comparer)
	{
		criticalSectionLocks = new Dictionary<TKey, CriticalSectionLock>(comparer);
	}

	/// <summary>
	/// Enters the critical section and returns a disposable object that releases the lock when disposed.
	/// </summary>
	/// <param name="lockValue">The value to lock on.</param>
	/// <returns>A disposable object that releases the lock when disposed.</returns>
	public IDisposable EnterScope(TKey lockValue)
	{
		// enter the critical section
		CriticalSectionLock criticalSectionLock = GetCriticalSectionLock(lockValue);
		criticalSectionLock.Semaphore.Wait();

		// when disposed, exit the critical section
		return new Scope(() =>
		{
			criticalSectionLock.Semaphore.Release();
			ReleaseCriticalSectionLock(lockValue, criticalSectionLock);
		});
	}

	/// <summary>
	/// Enters the critical section asynchronously and returns a disposable object that releases the lock when disposed.
	/// </summary>
	/// <param name="lockValue">The value to lock on.</param>
	/// <param name="cancellationToken">A cancellation token to observe while waiting for the lock.</param>
	/// <returns>A disposable object that releases the lock when disposed.</returns>
	public async Task<IDisposable> EnterScopeAsync(TKey lockValue, CancellationToken cancellationToken)
	{
		// enter the critical section
		CriticalSectionLock criticalSectionLock = GetCriticalSectionLock(lockValue);
		await criticalSectionLock.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

		// when disposed, exit the critical section
		return new Scope(() =>
		{
			criticalSectionLock.Semaphore.Release();
			ReleaseCriticalSectionLock(lockValue, criticalSectionLock);
		});
	}

	/// <summary>
	/// Executes the given action under the lock.
	/// </summary>
	/// <param name="lockValue">
	/// Lock. 
	/// Unlike an ordinary lock (or Monitor), it locks over its value, not its instance.
	/// Therefore, the lock can be anything that correctly implements the comparison operator (string, business object, ...).
	/// </param>
	/// <param name="criticalSection">The code of the critical section executed under the lock.</param>
	public void ExecuteAction(TKey lockValue, Action criticalSection)
	{
		using (EnterScope(lockValue))
		{
			criticalSection();
		}
	}

	/// <summary>
	/// Executes the given action under the lock.
	/// </summary>
	/// <param name="lockValue">
	/// Lock. 
	/// Unlike an ordinary lock (or Monitor), it locks over its value, not its instance.
	/// Therefore, the lock can be anything that correctly implements the comparison operator (string, business object, ...).
	/// </param>
	/// <param name="criticalSection">The code of the critical section executed under the lock.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	public async Task ExecuteActionAsync(TKey lockValue, Func<Task> criticalSection, CancellationToken cancellationToken = default)
	{
		using (await EnterScopeAsync(lockValue, cancellationToken).ConfigureAwait(false))
		{
#pragma warning disable CAC001 // ConfigureAwaitChecker
			await criticalSection();
#pragma warning restore CAC001 // ConfigureAwaitChecker
		}
	}

	internal CriticalSectionLock GetCriticalSectionLock(TKey lockValue)
	{
		// We are working with a counter, so we need to use a "global" lock.
		lock (criticalSectionLocks) // use a dictionary for the lock
		{
			if (criticalSectionLocks.TryGetValue(lockValue, out CriticalSectionLock criticalSectionLock))
			{
				criticalSectionLock.UsageCounter += 1;
			}
			else
			{
				criticalSectionLock = new CriticalSectionLock(); // the default value of the counter is 1
				Debug.Assert(criticalSectionLock.UsageCounter == 1);
				criticalSectionLocks.Add(lockValue, criticalSectionLock);
			}
			return criticalSectionLock;
		}
	}

	internal void ReleaseCriticalSectionLock(TKey lockValue, CriticalSectionLock criticalSectionLock)
	{
		// Whether the critical section ran well or an exception occurred, we need to decrease the usage counter of the lock.
		// Again, we are working with a counter, so we need to use a "global" lock.
		lock (criticalSectionLocks) // use a dictionary for the lock
		{
			criticalSectionLock.UsageCounter -= 1;
			if (criticalSectionLock.UsageCounter == 0)
			{
				// if no one is using the lock anymore, we clean it up
				criticalSectionLocks.Remove(lockValue);
				criticalSectionLock.Semaphore.Dispose();
			}
		}
	}

	/// <summary>
	/// Holds a lock (semaphore) with a usage counter.
	/// </summary>
	internal class CriticalSectionLock
	{
		/// <summary>
		/// Lock (semaphore).
		/// </summary>
		public SemaphoreSlim Semaphore { get; set; } = new SemaphoreSlim(1, 1);

		/// <summary>
		/// Usage counter. The default value is 1.
		/// </summary>
		public int UsageCounter { get; set; } = 1;
	}

	internal class Scope : IDisposable
	{
		private readonly Action _disposeAction;

		public Scope(Action disposeAction)
		{
			_disposeAction = disposeAction;
		}

		void IDisposable.Dispose()
		{
			_disposeAction();
		}
	}
}
