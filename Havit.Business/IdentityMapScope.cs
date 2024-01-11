using System;
using Havit.Scopes;

namespace Havit.Business;

/// <summary>
/// <see cref="Scope{T}"/> pro <see cref="IdentityMap"/>.
/// </summary>
public class IdentityMapScope : Scope<IdentityMap>
{
#if NETFRAMEWORK
	private static IScopeRepository<IdentityMap> scopeRepository = new Havit.Business.Scopes.WebApplicationScopeRepository<IdentityMap>();
#endif
#if NETSTANDARD
	private static readonly IScopeRepository<IdentityMap> scopeRepository = new AsyncLocalScopeRepository<IdentityMap>();
#endif

	/// <summary>
	/// Repository pro uložení scopů IdentityMap.
	/// Implementováno jako WebApplicationScopeRepository pro .NET Framework a jako AsyncLocalScopeRepository pro .NET Core.
	/// </summary>
	public static IScopeRepository<IdentityMap> ScopeRepository
	{
		get
		{
			return scopeRepository;
		}
		set
		{
			scopeRepository = value ?? throw new ArgumentNullException(nameof(value));
		}
	}

	/// <summary>
	/// Vrátí IdentityMapu pro aktuální scope.
	/// </summary>
	public static IdentityMap Current
	{
		get
		{
			return GetCurrent(ScopeRepository);
		}
	}

	/// <summary>
	/// Vytvoří <see cref="IdentityMapScope"/> obalující novou <see cref="IdentityMap"/>.
	/// </summary>
	public IdentityMapScope() : base(new IdentityMap(), ScopeRepository)
	{
	}
}
