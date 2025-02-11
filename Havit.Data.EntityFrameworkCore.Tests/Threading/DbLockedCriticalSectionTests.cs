using System.Diagnostics.Contracts;
using Havit.Data.EntityFrameworkCore.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Tests.Threading;

[TestClass]
public class DbLockedCriticalSectionTests
{
	[TestMethod]
	public void DbLockedCriticalSection_EnterScope_WorksWithNotOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Closed);
		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		using (criticalSection.EnterScope("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_EnterScopeAsync_WorksWithNotOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Closed);

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await using (await criticalSection.EnterScopeAsync("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public void DbLockedCriticalSection_EnterScope_WorksWithOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		sqlConnection.Open();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		using (criticalSection.EnterScope("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Open, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_EnterScopeAsync_WorksWithOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		await sqlConnection.OpenAsync();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await using (await criticalSection.EnterScopeAsync("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Open, sqlConnection.State);
	}

	[TestMethod]
	public void DbLockedCriticalSection_EnterScope_WorksWhenConnectionIsClosedInAction()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		using (criticalSection.EnterScope("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			sqlConnection.Close();
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_EnterScopeAsync_WorksWhenConnectionIsClosedInAction()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await using (await criticalSection.EnterScopeAsync("LOCK"))
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			await sqlConnection.CloseAsync();
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public void DbLockedCriticalSection_EnterScope_WorksWhenExceptionIsThrown()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		try
		{
			using (criticalSection.EnterScope("LOCK"))
			{
				throw new TestException();
			}
		}
		catch (TestException)
		{
			// NOOP
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_EnterScopeAsync_WorksWhenExceptionIsThrown()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		try
		{
			// Act
			await using (await criticalSection.EnterScopeAsync("LOCK"))
			{
				throw new TestException();
			}
		}
		catch (TestException)
		{
			// NOOP
		}

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	private SqlConnection CreateSqlConnection()
	{
		// chceme existující databázi, do které nebudeme zapisovat
		// nemáme zde migrace, které by databázi založili
		return new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=tempdb;Application Name=Havit.Data.EntityFrameworkCore.Tests");
	}

	private class TestException : Exception
	{
	}
}
