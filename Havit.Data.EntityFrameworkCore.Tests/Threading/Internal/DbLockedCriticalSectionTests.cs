using System.Diagnostics.Contracts;
using Havit.Data.EntityFrameworkCore.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Tests.Threading.Internal;

[TestClass]
public class DbLockedCriticalSectionTests
{
#pragma warning disable EF1001 // Internal EF Core API usage.

	[TestMethod]
	public void DbLockedCriticalSection_ExecuteAction_WorksWithNotOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Closed);
		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		criticalSection.ExecuteAction("LOCK", () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_ExecuteActionAsync_WorksWithNotOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Closed);

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await criticalSection.ExecuteActionAsync("LOCK", () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			return Task.CompletedTask;
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public void DbLockedCriticalSection_ExecuteAction_WorksWithOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		sqlConnection.Open();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		criticalSection.ExecuteAction("LOCK", () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Open, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_ExecuteActionAsync_WorksWithOpenedConnection()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();
		await sqlConnection.OpenAsync();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await criticalSection.ExecuteActionAsync("LOCK", () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			return Task.CompletedTask;
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Open, sqlConnection.State);
	}

	[TestMethod]
	public void DbLockedCriticalSection_ExecuteAction_WorksWhenConnectionIsClosedInAction()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		criticalSection.ExecuteAction("LOCK", () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			sqlConnection.Close();
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}

	[TestMethod]
	public async Task DbLockedCriticalSection_ExecuteActionAsync_WorksWhenConnectionIsClosedInAction()
	{
		// Arrange
		using SqlConnection sqlConnection = CreateSqlConnection();

		var criticalSection = new DbLockedCriticalSection(sqlConnection);

		// Act
		await criticalSection.ExecuteActionAsync("LOCK", async () =>
		{
			Contract.Assert(sqlConnection.State == System.Data.ConnectionState.Open);
			await sqlConnection.CloseAsync();
		});

		// Assert
		Assert.AreEqual(System.Data.ConnectionState.Closed, sqlConnection.State);
	}
#pragma warning restore EF1001 // Internal EF Core API usage.

	private SqlConnection CreateSqlConnection()
	{
		// chceme existující databázi, do které nebudeme zapisovat
		// nemáme zde migrace, které by databázi založili
		return new SqlConnection("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=tempdb;Application Name=Havit.Data.EntityFrameworkCore.Tests");
	}
}
