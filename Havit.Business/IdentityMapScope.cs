using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// <see cref="Scope{T}"/> pro <see cref="IdentityMap"/>.
	/// </summary>
	public class IdentityMapScope : Scope<IdentityMap>
	{
		#region Constructor
		/// <summary>
		/// Vytvoří <see cref="IdentityMapScope"/> obalující novou <see cref="IdentityMap"/>.
		/// </summary>
		public IdentityMapScope()
			: base(new IdentityMap(), true)
		{
		} 
		#endregion
	}
}
