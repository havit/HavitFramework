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
				((SqlConnection)connection).AccessToken = Havit.Data.SqlClient.Azure.SqlServerAadAuthentication.SqlConnectionFactory.GetAzureSqlAccessToken();
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
				((SqlConnection)connection).AccessToken = await Havit.Data.SqlClient.Azure.SqlServerAadAuthentication.SqlConnectionFactory.GetAzureSqlAccessTokenAsync(cancellationToken).ConfigureAwait(false);
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
							_shouldUseAadAuthentication = Havit.Data.SqlClient.Azure.SqlServerAadAuthentication.AadAuthenticationSupportDecision.ShouldUseAadAuthentication(sqlConnection.ConnectionString);
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
	}
}