using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kompozitní podmínka. Výsledek je pravdivý, jsou-li pravdivé všechny èlenské podmínky.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable]
	public class AndCondition: CompositeCondition
	{
		#region Constructor
		/// <summary>
		/// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
		/// </summary>		
		public AndCondition(params Condition[] conditions)
			: base("AND", conditions)
		{
		}
		#endregion
	}
}
