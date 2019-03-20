using System;
using System.Collections.Generic;
using System.Text;

using Havit.Data.SqlServer;

namespace Havit.Business.Query
{
	/// <summary>
	/// Interface podmínky dotazu.
	/// </summary>
	public abstract class Condition
	{
		/// <summary>
		/// Přidá část SQL příkaz pro sekci WHERE. Je VELMI doporučeno, aby byla podmínka přidána včetně závorek.
		/// </summary>
		public abstract void GetWhereStatement(System.Data.Common.DbCommand command, StringBuilder whereBuilder, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions);

		/// <summary>
		/// Udává, zda podmínka reprezentuje prázdnou podmínku, která nebude renderována (např. prázdná AndCondition).
		/// </summary>
		public abstract bool IsEmptyCondition();
	}
}
