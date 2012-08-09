using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kompozitní podmínka. Výsledek je pravdivý, jsou-li pravdivé všechny èlenské podmínky.
	/// </summary>
	[ComVisible(false)]
	[Serializable]
	public class AndCondition : CompositeCondition
	{
		#region Constructors
		/// <summary>
		/// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
		/// </summary>		
		public AndCondition(params Condition[] conditions)
			: base("AND", conditions)
		{
		}
		#endregion

        #region Create (static)
        /// <summary>
        /// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
        /// </summary>
        public static AndCondition Create(params Condition[] conditions)
        {
            return new AndCondition(conditions);
        } 
        #endregion
	}
}
