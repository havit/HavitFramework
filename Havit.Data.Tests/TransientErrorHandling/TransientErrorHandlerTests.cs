using System.Data.SqlClient;
using Havit.Data.TransientErrorHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Tests.TransientErrorHandling;

[TestClass]
public class TransientErrorHandlerTests
{
	[TestMethod]
	public void TransientErrorHandler_ExecuteAction_RetriesWithDelaysUpToMaxAttempts()
	{
		// Arrange
		SqlException sqlException = SqlExceptionHelper.CreateSqlException(10928);
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(sqlException)); // prerequisite

		// Act
		int maxAttempts = 3;
		TransientErrorRetryPolicy policy = new TransientErrorRetryPolicy(maxAttempts, new int[] { 0 });
		int i = 0;

		try
		{
			TransientErrorHandler.ExecuteAction<object>(
				() =>
				{
					i += 1;
					throw sqlException;
				},
				() => true,
				policy);
		}
		catch (Exception e)
		{
			if (e != sqlException)
			{
				throw;
			}
		}

		// Assert
		Assert.AreEqual(maxAttempts, i, "Max attempts");
	}

	[TestMethod]
	public void TransientErrorHandler_ExecuteAction_DoesNotRetryWhenNotPossible()
	{
		// Arrange
		SqlException sqlException = SqlExceptionHelper.CreateSqlException(10928);
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(sqlException)); // prerequisite

		// Act
		TransientErrorRetryPolicy policy = new TransientErrorRetryPolicy(3, new int[] { 0 });

		int i = 0;
		try
		{
			TransientErrorHandler.ExecuteAction<object>(
				() =>
				{
					i += 1;
					throw sqlException;
				},
				() => false, // nepovolujeme opakování
				policy);
		}
		catch (Exception e)
		{
			if (e != sqlException)
			{
				throw;
			}
		}

		// Assert
		Assert.AreEqual(1, i, "Attempts"); // nedošlo k opakování, přestože jde o transientní výjimku
	}
}
