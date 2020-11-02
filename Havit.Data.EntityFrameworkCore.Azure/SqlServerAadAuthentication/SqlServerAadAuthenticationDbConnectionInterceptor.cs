using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Azure.SqlServerAadAuthentication
{
	// inspirace: https://mderriey.com/2020/07/17/connect-to-azure-sql-with-aad-and-managed-identities/

	/// <summary>
	/// Allows use Managed Identity for Azure SQL Server.
	/// </summary>
	public class SqlServerAadAuthenticationDbConnectionInterceptor : DbConnectionInterceptor
	{
		/// <inheritdoc />
		public override InterceptionResult ConnectionOpening(DbConnection connection, ConnectionEventData eventData, InterceptionResult result)
		{
			if (ShouldUseAadAuthnetication(connection))
			{
				((SqlConnection)connection).AccessToken = GetAzureSqlAccessToken();
			}

			return base.ConnectionOpening(connection, eventData, result);
		}

		/// <inheritdoc />
		public override async Task<InterceptionResult> ConnectionOpeningAsync(
			DbConnection connection,
			ConnectionEventData eventData,
			InterceptionResult result,
			CancellationToken cancellationToken)
		{
			if (ShouldUseAadAuthnetication(connection))
			{
				((SqlConnection)connection).AccessToken = await GetAzureSqlAccessTokenAsync(cancellationToken).ConfigureAwait(false);
			}

			return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken).ConfigureAwait(false);
		}

		private bool ShouldUseAadAuthnetication(DbConnection connection)
		{
			if (_shouldUseAadAuthentication == null)
			{
				lock (typeof(SqlServerAadAuthenticationDbConnectionInterceptor))
				{
					if (_shouldUseAadAuthentication == null)
					{
						if (connection is SqlConnection sqlConnection)
						{
							var connectionStringBuilder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
							_shouldUseAadAuthentication = connectionStringBuilder.DataSource.ToLower().Contains("database.windows.net") && string.IsNullOrEmpty(connectionStringBuilder.UserID);
						}
						else
						{
							_shouldUseAadAuthentication = false;
						}
					}
				}
			}

			return _shouldUseAadAuthentication.Value;
		}
		private bool? _shouldUseAadAuthentication = null;

		// See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities#azure-sql
		private static string GetAzureSqlAccessToken()
		{
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = new DefaultAzureCredential().GetTokenAsync(tokenRequestContext).GetAwaiter().GetResult();

			return tokenRequestResult.Token;
		}

		private static async Task<string> GetAzureSqlAccessTokenAsync(CancellationToken cancellationToken)
		{
			var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
			var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(tokenRequestContext, cancellationToken).ConfigureAwait(false);

			return tokenRequestResult.Token;
		}
	}
}