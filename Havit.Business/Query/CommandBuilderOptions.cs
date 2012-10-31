using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Nastavenžadavků pro tvorbu databázových dotazů.
	/// </summary>
	[Flags]
	public enum CommandBuilderOptions
	{
		/// <summary>
		/// Žádná specifická nastavení pro command builder.
		/// </summary>
		None = 0,

		///// <summary>
		///// Test ověření "IN" bude řešen přes IN s výčtem hodnot, tj. WHERE xxx IN (1, 2, 3, 4, 5, 6, 7, 8, 9), atp.
		///// </summary>
		//ReferenceInAsEnumeration = 1,		
	}
}
