using Havit.Core;
using Havit.Diagnostics.Contracts;
using Microsoft.Data.SqlClient;

namespace Havit.Tests;

[TestClass]
public class CancellationExceptionCheckerTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	[DataRow("en-US")]
	[DataRow("cs-CZ")]
	//[DataRow("fr-FR")] - not supported -> test fails
	public async Task CancellationExceptionChecker_IsCancellationException_ReturnsTrueWhenSqlCommandCancelled(string culture)
	{
		// Arrange
		SqlException sqlException = null;

		// we need to simulate SqlException caused by command cancellation		
		var sqlConnection = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=master;Application Name=Havit.Core.Tests;ConnectRetryCount=0");
		await sqlConnection.OpenAsync(TestContext.CancellationToken);

		try
		{
			var sqlCommand = sqlConnection.CreateCommand();

			sqlCommand.CommandText = "PRINT 1;"; // warm-up
			await sqlCommand.ExecuteNonQueryAsync(TestContext.CancellationToken);

			sqlCommand.CommandText = "WAITFOR DELAY '00:00:10';"; // simulate a long-running query
			CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
			cts.CancelAfter(10); // 10 milliseconds

			try
			{
				using (var cultureInfoScope = CultureInfoExt.EnterScope(new System.Globalization.CultureInfo(culture))) // set localization
				{
					await sqlCommand.ExecuteNonQueryAsync(cts.Token); // execute the sql command but cancel it after 10 ms
				}
			}
			catch (SqlException catchedSqlException)
			{
				sqlException = catchedSqlException;
			}
		}
		finally
		{
#if NETFRAMEWORK
			sqlConnection.Close();
#else
			await sqlConnection.CloseAsync();
#endif
		}

		Contract.Assert(sqlException != null);

		// Act + Assert
		Assert.IsTrue(CancellationExceptionChecker.IsCancellationException(sqlException));
	}
}
