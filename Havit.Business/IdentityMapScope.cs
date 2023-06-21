using System;
using System.Collections.Generic;
using System.Text;
using Havit.Scopes;

namespace Havit.Business;

/// <summary>
/// <see cref="Scope{T}"/> pro <see cref="IdentityMap"/>.
/// </summary>
public class IdentityMapScope : Scope<IdentityMap>
{
	/// <summary>
	/// Repository pro uložení scopů IdentityMap.
	/// Implementováno jako WebApplicationScopeRepository pro .NET Framework a jako AsyncLocalScopeRepository pro .NET Core. Prozatím bez možnosti nastavení.
	/// </summary>
#if NETFRAMEWORK
	private static readonly IScopeRepository<IdentityMap> repository = new Havit.Business.Scopes.WebApplicationScopeRepository<IdentityMap>();
#endif
#if NETSTANDARD
	private static readonly IScopeRepository<IdentityMap> repository = new AsyncLocalScopeRepository<IdentityMap>();
#endif

	/// <summary>
	/// Vrátí IdentityMapu pro aktuální scope.
	/// </summary>
	public static IdentityMap Current
	{
		get
		{
			return GetCurrent(repository);
		}
	}

	/// <summary>
	/// Vytvoří <see cref="IdentityMapScope"/> obalující novou <see cref="IdentityMap"/>.
	/// </summary>
	public IdentityMapScope() : base(new IdentityMap(), repository)
	{
	}
}
