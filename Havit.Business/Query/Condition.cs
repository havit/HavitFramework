using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Query
{
	/// <summary>
	/// Interface podmínky dotazu.
	/// </summary>
	public abstract class Condition
	{
		#region GetWhereStatement
		/// <summary>
		/// Přidá část SQL příkaz pro sekci WHERE. Je VELMI doporučeno, aby byla podmínka přidána včetně závorek.
		/// </summary>
		public abstract void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder); 
		#endregion

		#region IsEmptyCondition
		/// <summary>
		/// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována (např. prázdná AndCondition).
		/// </summary>
		public abstract bool IsEmptyCondition(); 
		#endregion
	}
}
