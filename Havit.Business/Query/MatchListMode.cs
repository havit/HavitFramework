using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Určuje způsob tvorby dotazu na více hodnot (IN, NOT IN).
	/// </summary>
	[Obsolete]
	public enum MatchListMode
	{
		/// <summary>
		/// Podmínka je tvořena způsobem: WHERE Field IN IN (SELECT Value FROM dbo.IntArrayToTable(@parameter).
		/// </summary>
		IntArray,

		/// <summary>
		/// Podmínka je tvořena způsobem: WHERE Field IN (1, 2, 3, 4).
		/// </summary>
		CommaSeparatedList
	}
}
