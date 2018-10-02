using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Havit.Business.BusinessLayerGenerator.Settings
{
	public static class ConnectionParametersParser
	{
		private static readonly string ConnectionStringName = "DefaultConnectionString";

		public static ConnectionParameters ParseParametersFromWebConfig(string webConfig)
		{
			var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = webConfig }, ConfigurationUserLevel.None);

			var connectionString = configuration.ConnectionStrings.ConnectionStrings[ConnectionStringName]?.ConnectionString;
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException($"Invalid connection string ({ConnectionStringName}): {connectionString}");
			}

			return ParseParameters(connectionString);
		}

		private static ConnectionParameters ParseParameters(string connectionString)
		{
			var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
			return new ConnectionParameters
			{
				ServerName = connectionStringBuilder.DataSource,
				Username = connectionStringBuilder.UserID,
				Password = connectionStringBuilder.Password,
				DatabaseName = connectionStringBuilder.InitialCatalog
			};
		}
	}
}