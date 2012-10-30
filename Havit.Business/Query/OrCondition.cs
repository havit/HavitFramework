using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kompozitní podmínka. Výsledek je pravdivý, je-li pravdivá alespoň jedna členská podmínka.
	/// </summary>
	public class OrCondition : CompositeCondition
	{
        #region Constructors
        /// <summary>
        /// Vytvoří kompozitní podmínku. Lze inicializovat sadou členských podmínek.
        /// </summary>		
        public OrCondition(params Condition[] conditions)
            : base("OR", conditions)
        {
        }
        #endregion

        #region Create (static)
        /// <summary>
        /// Vytvoří kompozitní podmínku. Lze inicializovat sadou členských podmínek.
        /// </summary>
        public static OrCondition Create(params Condition[] conditions)
        {
            return new OrCondition(conditions);
        }
        #endregion

	}
}
