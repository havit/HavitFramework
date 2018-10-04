using System;
using System.Collections.Generic;
using System.Text;
using Havit.Business.Scopes;
using Havit.Scopes;

namespace Havit.Business
{
	/// <summary>
	/// <see cref="Scope{T}"/> pro <see cref="IdentityMap"/>.
	/// </summary>
	public class IdentityMapScope : Scope<IdentityMap>
	{
		#region repository (Private field)
		/// <summary>
		/// Repository pro uložení scopů IdentityMap.
		/// Implementováno jako WebApplicationScopeRepository, prozatím bez možnosti nastavení.
		/// </summary>
		private static readonly IScopeRepository<IdentityMap> repository = new WebApplicationScopeRepository<IdentityMap>();
		#endregion

		#region Current
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
		#endregion

		#region Constructor
		/// <summary>
		/// Vytvoří <see cref="IdentityMapScope"/> obalující novou <see cref="IdentityMap"/>.
		/// </summary>
		public IdentityMapScope() : base(new IdentityMap(), repository)
		{
		} 
		#endregion

	}
}
