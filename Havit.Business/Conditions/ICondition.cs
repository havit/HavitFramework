using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business.Conditions
{
	/// <summary>
	/// Interface podmínky dotazu.
	/// </summary>
	public interface ICondition
	{
		/// <summary>
		/// Pøidá èást SQL pøíkaz pro sekci WHERE. Je VELMI doporuèeno, aby byla podmínka pøidána vèetnì závorek.
		/// </summary>
		void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder);
	}
}
