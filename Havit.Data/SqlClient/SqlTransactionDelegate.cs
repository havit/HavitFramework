using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Havit.Data.SqlClient
{
	/// <summary>
	/// Reprezentuje metodu, která vykonává jednotlivé kroky transakce.
	/// </summary>
	/// <param name="transaction">transakce, v rámci které mají být jednotlivé kroky vykonány</param>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public delegate void SqlTransactionDelegate(SqlTransaction transaction);
}
