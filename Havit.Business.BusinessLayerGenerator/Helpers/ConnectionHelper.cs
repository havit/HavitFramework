using Havit.Business.BusinessLayerGenerator.Settings;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class ConnectionHelper
	{
		public static SqlDataReader GetDataReader(SqlCommand command)
		{
			SqlConnection connection;

            if (String.IsNullOrEmpty(GeneratorSettings.Username))
			{
				connection = new SqlConnection(
					String.Format("Data Source = {0}; Initial Catalog = {1}; Integrated Security=SSPI;",
					    GeneratorSettings.SqlServerName, GeneratorSettings.DatabaseName));
			}
			else
			{
				connection = new SqlConnection(
					String.Format("Data Source = {0}; Initial Catalog = {1}; User ID={2};Pwd={3}",
					    GeneratorSettings.SqlServerName, GeneratorSettings.DatabaseName, GeneratorSettings.Username, GeneratorSettings.Password));
			}
			connection.Open();

			command.Connection = connection;
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}
	}
}
