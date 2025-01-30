namespace Havit.Scopes;

/// <summary>
/// Interface for classes implementing the scope repository - a repository that allows storing and loading a value into the scope.
/// </summary>
/// <typeparam name="T">The type whose scope is stored in the repository.</typeparam>
public interface IScopeRepository<T>
	where T : class
{
	/// <summary>
	/// Returns the value of the current scope.
	/// </summary>
	Scope<T> GetCurrentScope();

	/// <summary>
	/// Sets the value of the current scope.
	/// </summary>
	void SetCurrentScope(Scope<T> scope);

	/// <summary>
	/// Removes the scope.
	/// </summary>
	void RemoveCurrentScope();

}
