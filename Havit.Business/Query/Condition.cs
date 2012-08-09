using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Interface podmínky dotazu.
	/// </summary>
	[Serializable]
	public abstract class Condition
	{
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE. Je VELMI doporuèeno, aby byla podmínka pøidána vèetnì závorek.
		/// </summary>
		public abstract void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder);

		/// <summary>
		/// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována (napø. prázdná AndCondition).
		/// </summary>
		public abstract bool IsEmptyCondition();
	}
}
