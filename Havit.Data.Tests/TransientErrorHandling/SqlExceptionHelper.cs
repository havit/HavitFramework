using System.Data.SqlClient;
using System.Reflection;

namespace Havit.Data.Tests.TransientErrorHandling;

internal static class SqlExceptionHelper
{
	internal static SqlException CreateSqlException(int errorNumber, string message = null)
	{
		SqlError sqlError = (SqlError)typeof(SqlError)
			.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
				new Type[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int) },
				null)
			.Invoke(new object[] { errorNumber, (byte)0, (byte)0, "0", "0", null, 0 });

		// constructor je internal
		SqlErrorCollection sqlErrorCollection = (SqlErrorCollection)typeof(SqlErrorCollection)
			.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { }, null)
			.Invoke(new object[] { });

		// Add je internal
		sqlErrorCollection
			.GetType()
			.GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic)
			.Invoke(sqlErrorCollection, new object[] { sqlError });

		// constructor SqlException je internal
		SqlException sqlException = (SqlException)typeof(SqlException)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) }, null)
			.Invoke(new object[] { message ?? String.Empty, sqlErrorCollection, null, Guid.NewGuid() });
		return sqlException;
	}
}
