using Havit.Core;
using Microsoft.Data.SqlClient;

namespace Havit.Tests;

[TestClass]
public class CancellationExceptionCheckerTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void CancellationExceptionChecker_IsCancellationException_ReturnsTrueForTaskCanceledException()
	{
		// Act + Assert
		Assert.IsTrue(CancellationExceptionChecker.IsCancellationException(new TaskCanceledException()));
	}

	[TestMethod]
	public void CancellationExceptionChecker_IsCancellationException_ReturnsTrueForOperationCanceledException()
	{
		// Act + Assert
		Assert.IsTrue(CancellationExceptionChecker.IsCancellationException(new OperationCanceledException()));
	}

	[TestMethod]
	[DataRow("en-US")]
	[DataRow("cs-CZ")]
	//[DataRow("fr-FR")] - not supported -> test fails
	public async Task CancellationExceptionChecker_IsCancellationException_ReturnsTrueWhenSqlCommandCancelled(string culture)
	{
		// Arrange
		using (var cultureInfoScope = CultureInfoExt.EnterScope(new System.Globalization.CultureInfo(culture))) // set localization
		{
			SqlException sqlException = null;

			// we need to simulate SqlException caused by command cancellation		
			using (var sqlConnection = new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=master;Application Name=Havit.Core.Tests;ConnectRetryCount=0"))
			{
				await sqlConnection.OpenAsync(TestContext.CancellationToken);

				using (var sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandText = "PRINT 1;"; // warm-up
					sqlCommand.CommandTimeout = 3600 /* 1 hour */;
					await sqlCommand.ExecuteNonQueryAsync(TestContext.CancellationToken);
				}

				using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
				{
					sqlCommand.CommandText = "WAITFOR DELAY '01:00:00';"; // simulate a long-running query
					sqlCommand.CommandTimeout = 3600 /* 1 hour */;

					CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
					cts.CancelAfter(10); // 10 milliseconds

					// Act
					// Execute the sql command but cancel it after 10 ms.
					sqlException = await Assert.ThrowsAsync<SqlException>(async () => await sqlCommand.ExecuteNonQueryAsync(cts.Token));
				}
			}

			// Assert
			Assert.IsTrue(CancellationExceptionChecker.IsCancellationException(sqlException),
				message: $"SqlException was expected to be recognized as cancellation exception, but it was not. Exception details: {sqlException}");
		}
	}
}
