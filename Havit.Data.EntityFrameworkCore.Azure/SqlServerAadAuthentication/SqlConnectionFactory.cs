using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Azure.SqlServerAadAuthentication
{
	/// <summary>
	/// SqlConnection factory for instances (optionally) supporting AAD authentication.
	/// </summary>
	/// <remarks>
	/// This class might be moved to another assembly in future.
	/// </remarks>
	public class SqlConnectionFactory
	{
		/// <summary>
		/// Returns SqlConnection with optional AAD authentication support.
		/// </summary>
		public static SqlConnection CreateSqlConnectionWithAadSupport(string connectionString)
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
		public static async ValueTask<SqlConnection> CreateSqlConnectionWithAadSupportAsync(string connectionString)
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
		internal static string GetAzureSqlAccessToken()
		{
			// See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities#azure-sql
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = new DefaultAzureCredential().GetTokenAsync(tokenRequestContext).ConfigureAwait(false).GetAwaiter().GetResult();

			return tokenRequestResult.Token;
		}

		/// <summary>
		/// Returns the token for access Azure Sql Database using Aad Managed Identity
		/// </summary>
		internal static async ValueTask<string> GetAzureSqlAccessTokenAsync(CancellationToken cancellationToken = default)
		{
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

			return tokenRequestResult.Token;
		}
	}
}
