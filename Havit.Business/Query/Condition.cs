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
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE. Je VELMI doporuèeno, aby byla podmínka pøidána vèetnì závorek.
		/// </summary>
		public abstract void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder);
	}
}
