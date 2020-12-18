using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.SqlClient.Azure.SqlServerAadAuthentication
{
	/// <summary>
	/// SqlConnection factory for instances (optionally) supporting AAD authentication.
	/// Use for ASP.NET Core applications with System.Data.SqlClient (like EF 6 on ASP.NET Core).
	/// For ASP.NET Framework application see https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi#modify-aspnet
	/// </summary>
	/// <remarks>
	/// https://docs.microsoft.com/en-us/azure/app-service/app-service-web-tutorial-connect-msi
	/// </remarks>
	public class SqlConnectionFactory
	{
		/// <summary>
		/// Returns SqlConnection with optional AAD authentication support.
		/// </summary>
		public static SqlConnection CreateSqlConnectionWithAadAuthenticationSupport(string connectionString)
		{
			var sqlConnection = new SqlConnection(connectionString);
			if (AadAuthenticationSupportDecision.ShouldUseAadAuthentication(connectionString))
			{
				sqlConnection.AccessToken = GetAzureSqlAccessToken();
			}
			return sqlConnection;
		}

		/// <summary>
		/// Returns SqlConnection with optional AAD authentication support.
		/// </summary>
		public static async ValueTask<SqlConnection> CreateSqlConnectionWithAadAuthenticationSupportAsync(string connectionString, CancellationToken cancellationToken = default)
		{
			var sqlConnection = new SqlConnection(connectionString);
			if (AadAuthenticationSupportDecision.ShouldUseAadAuthentication(connectionString))
			{
				sqlConnection.AccessToken = await GetAzureSqlAccessTokenAsync(cancellationToken).ConfigureAwait(false);
			}
			return sqlConnection;
		}

		/// <summary>
		/// Returns the token for access Azure Sql Database using Aad Managed Identity
		/// </summary>
		public static string GetAzureSqlAccessToken()
		{
			return (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").ConfigureAwait(false).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Returns the token for access Azure Sql Database using Aad Managed Identity
		/// </summary>
		public static async ValueTask<string> GetAzureSqlAccessTokenAsync(CancellationToken cancellationToken = default)
		{
			return await (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/", cancellationToken: cancellationToken).ConfigureAwait(false);
		}
	}
}
