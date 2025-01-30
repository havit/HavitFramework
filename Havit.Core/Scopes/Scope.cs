using System.Diagnostics;
using Havit.Diagnostics.Contracts;

namespace Havit.Scopes;

/// <summary>
/// Thread-specific Scope wrapping the scope of a specific object (transaction, identity map, etc.),
/// which is subsequently accessible through the GetCurrent property method (method intended for use in descendants to the public Current property).
/// </summary>
/// <example>
/// <code>
/// using (new Scope&lt;IdentityMap&gt;(new IdentityMap()))
/// {
///		Console.WriteLine(Scope.Current.SomeMethod("outer scope"));
/// 
///		using (new Scope&lt;IdentityMap&gt;(new IdentityMap()))
///		{
///			Console.WriteLine(Scope.Current.SomeMethod("inner scope"));
///		}
///		
///		Console.WriteLine(Scope.Current.SomeMethod("inner scope"));
///	}
/// </code>
/// </example>
/// <remarks>
/// Implementation based on the MSDN Magazine article <a href="http://msdn.microsoft.com/msdnmag/issues/06/09/netmatters/default.aspx">Stephen Toub: Scope&lt;T&gt; and More</a> (no longer available).
/// </remarks>
/// <typeparam name="T">type of the object whose scope we are addressing</typeparam>
public class Scope<T> : IDisposable
	where T : class
{
	/// <summary>
	/// Current instance wrapped by the scope.
	/// Intended for use in descendants to implement the static Current property.
	/// </summary>
	/// <param name="scopeRepository">scope reading repository</param>
	protected static T GetCurrent(IScopeRepository<T> scopeRepository)
	{
		Scope<T> scope = scopeRepository.GetCurrentScope();
		return scope != null ? scope.instance : null;
	}

	/// <summary>
	/// Indicates whether the class has already been disposed.
	/// </summary>
	private bool disposed;

	/// <summary>
	/// Indicates whether the instance is owned by the scope, i.e., whether we should dispose of it at the end of the scope.
	/// </summary>
	private readonly bool ownsInstance;

	/// <summary>
	/// The instance wrapped by the scope.
	/// </summary>
	private readonly T instance;

	/// <summary>
	/// The parent scope in the linked list of nested scopes.
	/// </summary>
	private readonly Scope<T> parent;

	private readonly IScopeRepository<T> owningScopeRepository;

	/// <summary>
	/// Creates an instance of the <see cref="Scope{T}"/> class around the instance. The instance will also be disposed of when the Scope is disposed.
	/// </summary>
	/// <param name="instance">the instance wrapped by the scope</param>
	/// <param name="scopeRepository">scope storage repository</param>
	protected Scope(T instance, IScopeRepository<T> scopeRepository) : this(instance, scopeRepository, true) { }

	/// <summary>
	/// Creates an instance of the <see cref="Scope{T}"/> class around the instance.
	/// </summary>
	/// <param name="instance">the instance wrapped by the scope</param>
	/// <param name="scopeRepository">scope storage repository</param>
	/// <param name="ownsInstance">indicates whether we own the instance, i.e., whether we should dispose of it at the end of the scope</param>
	protected Scope(T instance, IScopeRepository<T> scopeRepository, bool ownsInstance)
	{
		Contract.Requires<ArgumentNullException>(instance != null, nameof(instance));

		this.instance = instance;
		this.owningScopeRepository = scopeRepository;
		this.ownsInstance = ownsInstance;

		// linked list for nesting scopes
		this.parent = scopeRepository.GetCurrentScope();
		scopeRepository.SetCurrentScope(this);
	}

	/// <summary>
	/// Ends the scope and disposes of the owned instances.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		// no unmanaged resources owned, not needed: Finalize() + GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Dispose. Releases the object representing the scope (calls Dispose methods) if this scope implements IDisposable.
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			disposed = true;

			Debug.Assert(this == owningScopeRepository.GetCurrentScope(), "Disposed out of order.");
			if (parent == null)
			{
				owningScopeRepository.RemoveCurrentScope();
			}
			else
			{
				owningScopeRepository.SetCurrentScope(parent);
			}

			if (ownsInstance)
			{
				IDisposable disposable = instance as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
