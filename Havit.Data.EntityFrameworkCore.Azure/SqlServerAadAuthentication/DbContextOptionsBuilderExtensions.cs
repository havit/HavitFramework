using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Azure.SqlServerAadAuthentication
{
	/// <summary>
	/// DbContextOptionsBuilder Extension methods.
	/// </summary>
	public static class DbContextOptionsBuilderExtensions
	{
		/// <summary>
		/// Allows use AAD authentication for SqlServer. Used for accessing Azure SQL Server using Managed Identity.
		/// AAD authentication is used when there is no "User ID" provided in connections string and the data source contains "database.windows.net".
		/// Otherwise the AAD functionality is not added.		
		/// The functionality expects the application uses just one connection string for one DbContext (connection string cannot be altered at runtime).
		/// </summary>
		public static DbContextOptionsBuilder UseSqlServerAadAuthentication(this DbContextOptionsBuilder optionsBuilder)
		{
			return optionsBuilder.AddInterceptors(new SqlServerAadAuthenticationDbConnectionInterceptor());
		}
	}
}
