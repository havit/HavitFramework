using Havit.Scopes;

namespace Havit.Tests.Scopes.Instrastructure;

internal class TestAsyncLocalScope : Scope<object>
{
	private static readonly IScopeRepository<object> repository = new AsyncLocalScopeRepository<object>();

	public TestAsyncLocalScope(object instance) : base(instance, repository)
	{
	}

	public static object Current
	{
		get
		{
			return GetCurrent(repository);
		}
	}
}
