namespace Havit.Data.Tests.TransientErrorHandling;

[TestClass]
public class SqlDatabaseTransientErrorDetectionStrategyTests
{
	[TestMethod]
	public void SqlDatabaseTransientErrorDetectionStrategy_IsTransient()
	{
		// Act + Assert
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(40501)), "40501");
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(10928)), "10928");
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(10929)), "10929");
		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(10054)), "10054");

		Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(-2, "Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding.  This failure occurred while attempting to connect to the routing destination. The duration spent while attempting to connect to the original server was - [Pre-Login] initialization=54; handshake=215; [Login] initialization=14; authentication=0; [Post-Login] complete=1")));
		Assert.IsFalse(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(SqlExceptionHelper.CreateSqlException(-2)));
	}
}
