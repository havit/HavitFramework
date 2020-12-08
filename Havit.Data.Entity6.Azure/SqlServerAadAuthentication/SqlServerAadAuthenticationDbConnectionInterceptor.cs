using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;

namespace Havit.Data.Entity.Azure.SqlServerAadAuthentication
{
	// inspirace: https://mderriey.com/2020/07/17/connect-to-azure-sql-with-aad-and-managed-identities/

	/// <summary>
	/// Allows use Managed Identity for Azure SQL Server.
	/// </summary>
	public class SqlServerAadAuthenticationDbConnectionInterceptor : System.Data.Entity.Infrastructure.Interception.IDbConnectionInterceptor
	{
		/// <inheritdoc />
		public void Opening(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			if (ShouldUseAadAuthnetication(connection))
			{
				((SqlConnection)connection).AccessToken = Havit.Data.SqlClient.Azure.SqlServerAadAuthentication.SqlConnectionFactory.GetAzureSqlAccessToken();
			}
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

		#region Empty interface implementation
		/// <inheritdoc />
		public void BeganTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void BeginningTransaction(DbConnection connection, BeginTransactionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void Closing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionStringGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionStringGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionStringSet(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionStringSetting(DbConnection connection, DbConnectionPropertyInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionTimeoutGetting(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ConnectionTimeoutGot(DbConnection connection, DbConnectionInterceptionContext<int> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void DatabaseGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void DatabaseGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void DataSourceGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void DataSourceGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void Disposed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void Disposing(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void EnlistedTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void EnlistingTransaction(DbConnection connection, EnlistTransactionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ServerVersionGetting(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void ServerVersionGot(DbConnection connection, DbConnectionInterceptionContext<string> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void StateGetting(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
		{
			// NOOP
		}

		/// <inheritdoc />
		public void StateGot(DbConnection connection, DbConnectionInterceptionContext<ConnectionState> interceptionContext)
		{
			// NOOP
		}
		#endregion
	}
}