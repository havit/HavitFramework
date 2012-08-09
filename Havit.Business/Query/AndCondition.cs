using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kompozitní podmínka. Výsledek je pravdivý, jsou-li pravdivé všechny èlenské podmínky.
	/// </summary>
	[Serializable]
	public class AndCondition: CompositeCondition
	{
		#region Constructor
		/// <summary>
		/// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
		/// </summary>		
		public AndCondition(params ICondition[] conditions)
			: base("AND", conditions)
		{
		}
		#endregion
	}
}
