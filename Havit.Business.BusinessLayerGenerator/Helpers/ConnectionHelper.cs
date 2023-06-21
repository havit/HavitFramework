using System.Data;
using System.Data.SqlClient;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

public static class ConnectionHelper
{
	public static SqlConnection SqlConnection { get; set; }

	public static SqlDataReader GetDataReader(SqlCommand command)
	{
		command.Connection = SqlConnection;
		return command.ExecuteReader();
	}
}
