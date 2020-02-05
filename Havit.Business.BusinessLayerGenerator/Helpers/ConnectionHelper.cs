using System.Data;
using System.Data.SqlClient;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class ConnectionHelper
	{
		public static SqlDataReader GetDataReader(SqlCommand command)
		{
			command.Connection = DatabaseHelper.Database.Parent.ConnectionContext.SqlConnectionObject;
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}
	}
}
