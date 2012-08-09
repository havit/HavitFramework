using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Kompozitní podmínka. Výsledek je pravdivý, je-li pravdivá alespoò jedna èlenská podmínka.
	/// </summary>
	[Serializable]
	public class OrCondition: CompositeCondition
	{
        #region Constructors
        /// <summary>
        /// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
        /// </summary>		
        public OrCondition(params Condition[] conditions)
            : base("OR", conditions)
        {
        }
        #endregion

        #region Create (static)
        /// <summary>
        /// Vytvoøí kompozitní podmínku. Lze inicializovat sadou èlenských podmínek.
        /// </summary>
        public static OrCondition Create(params Condition[] conditions)
        {
            return new OrCondition(conditions);
        }
        #endregion

	}
}
