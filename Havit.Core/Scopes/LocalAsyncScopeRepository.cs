namespace Havit.Scopes;

/// <summary>
/// Repository implementing scope using AsyncLocal&lt;T&gt;.
/// </summary>
/// <typeparam name="T">The type whose scope is stored in the repository.</typeparam>
public class AsyncLocalScopeRepository<T> : IScopeRepository<T>
	where T : class
{
	private readonly AsyncLocal<Scope<T>> storage;

	/// <summary>
	/// Constructor.
	/// </summary>
	public AsyncLocalScopeRepository()
	{
		// initialize the data slot
		this.storage = new AsyncLocal<Scope<T>>();
	}

	/// <summary>
	/// Returns the value of the current scope.
	/// </summary>
	public Scope<T> GetCurrentScope()
	{
		return storage.Value;
	}

	/// <summary>
	/// Sets the value of the current scope.
	/// </summary>
	public void SetCurrentScope(Scope<T> scope)
	{
		storage.Value = scope;
	}

	/// <summary>
	/// Removes the scope.
	/// </summary>
	public void RemoveCurrentScope()
	{
		storage.Value = null;
	}
}
