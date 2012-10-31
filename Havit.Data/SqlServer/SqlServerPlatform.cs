using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Data.SqlServer
{
	/// <summary>
	/// Platforma (verze) Microsoft SQL Serveru.
	/// </summary>
	public enum SqlServerPlatform
	{
		/// <summary>
		/// Microsoft SQL Server Compact Edition 3.5
		/// </summary>
		SqlServerCe35 = -1,

		/// <summary>
		/// Microsoft SQL Server 2005
		/// </summary>
		SqlServer2005 = 5,

		/// <summary>
		/// Microsoft SQL Server 2008
		/// </summary>
		SqlServer2008 = 8,

		/// <summary>
		/// Microsoft SQL Server 2012
		/// </summary>
		SqlServer2012 = 12
	}
}
