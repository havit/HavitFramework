using Azure.Core;
using Azure.Identity;
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
	/// </summary>
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
		public static async ValueTask<SqlConnection> CreateSqlConnectionWithAadAuthenticationSupportAsync(string connectionString)
		{
			var sqlConnection = new SqlConnection(connectionString);
			if (AadAuthenticationSupportDecision.ShouldUseAadAuthentication(connectionString))
			{
				sqlConnection.AccessToken = await GetAzureSqlAccessTokenAsync().ConfigureAwait(false);
			}
			return sqlConnection;
		}

		/// <summary>
		/// Returns the token for access Azure Sql Database using Aad Managed Identity
		/// </summary>
		public static string GetAzureSqlAccessToken()
		{
			// See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities#azure-sql
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = new DefaultAzureCredential().GetTokenAsync(tokenRequestContext).ConfigureAwait(false).GetAwaiter().GetResult();

			return tokenRequestResult.Token;
		}

		/// <summary>
		/// Returns the token for access Azure Sql Database using Aad Managed Identity
		/// </summary>
		public static async ValueTask<string> GetAzureSqlAccessTokenAsync(CancellationToken cancellationToken = default)
		{
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

			return tokenRequestResult.Token;
		}
	}
}
