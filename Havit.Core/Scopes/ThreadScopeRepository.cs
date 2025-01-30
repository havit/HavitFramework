namespace Havit.Scopes;

/// <summary>
/// Repository implementing scope as thread data (Thread.GetData, ThreadSetData).
/// </summary>
/// <typeparam name="T">The type whose scope is stored in the repository.</typeparam>
public class ThreadScopeRepository<T> : IScopeRepository<T>
	where T : class
{
	/// <summary>
	/// DataSlot - unnamed slot under which thread data is stored.
	/// </summary>
	private readonly LocalDataStoreSlot threadDataStoreSlot;

	/// <summary>
	/// Constructor.
	/// </summary>
	public ThreadScopeRepository()
	{
		// initialize the data slot
		this.threadDataStoreSlot = Thread.AllocateDataSlot();
	}

	/// <summary>
	/// Returns the value of the current scope.
	/// </summary>
	public Scope<T> GetCurrentScope()
	{
		return (Scope<T>)Thread.GetData(this.threadDataStoreSlot);
	}

	/// <summary>
	/// Sets the value of the current scope.
	/// </summary>
	public void SetCurrentScope(Scope<T> scope)
	{
		Thread.SetData(this.threadDataStoreSlot, scope);
	}

	/// <summary>
	/// Removes the scope.
	/// </summary>
	public void RemoveCurrentScope()
	{
		Thread.SetData(this.threadDataStoreSlot, null);
	}
}
