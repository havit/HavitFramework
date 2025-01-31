using System.Data.SqlClient;

namespace Havit.Data.SqlClient.Azure.SqlServerAadAuthentication;

/// <summary>
/// Decides whether to use AAD authentication ow whether not to use it.
/// </summary>
public static class AadAuthenticationSupportDecision
{
	/// <summary>
	/// Returns true if connections string is in format which suggests us to use AAD Authentication.
	/// </summary>
	public static bool ShouldUseAadAuthentication(string connectionString)
	{
		var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
		return connectionStringBuilder.DataSource.ToLower().Contains("database.windows.net") && string.IsNullOrEmpty(connectionStringBuilder.UserID);
	}
}
