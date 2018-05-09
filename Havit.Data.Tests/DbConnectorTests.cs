using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Havit.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Tests
{
	/// <summary>
	/// This is a test class for Havit.Data.DbConnector and is intended
	/// to contain all Havit.Data.DbConnector Unit Tests
	/// </summary>
	[TestClass]
	public class DbConnectorTests
	{
		/// <summary>
		/// A test for DbConnector (ConnectionStringSettings)
		/// </summary>
		[TestMethod]
		public void DbConnector_Constructor_ConnectionStringSettings()
		{
			ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["Test"];
			DbConnector target = new DbConnector(connectionStringSettings);

            Assert.AreEqual(target.ConnectionString, connectionStringSettings.ConnectionString);
            Assert.AreEqual(target.ProviderFactory.GetType(), typeof(OleDbFactory));
		}

		/// <summary>
		/// A test for DbConnector (string, DbProviderFactory)
		/// </summary>
		[TestMethod]
		public void DbConnector_Constructor_StringAndDbProviderFactory()
		{
			string connectionString = "Data Source=dev;Initial Catalog=HavitTest;User Id=development;Password=development;";
			DbProviderFactory providerFactory = DbProviderFactories.GetFactory("System.Data.SqlClient");

			DbConnector target = new DbConnector(connectionString, providerFactory);

            Assert.AreEqual(target.ConnectionString, connectionString);
            Assert.AreEqual(target.ProviderFactory, providerFactory);
		}

		/// <summary>
		/// A test for DbConnector (string, string)
		/// </summary>
		[TestMethod]
		public void DbConnector_Constructor_StringAndString()
		{
			string connectionString = "Data Source=dev;Initial Catalog=HavitTest;User Id=development;Password=development;";
			string providerInvariantName = "System.Data.SqlClient";

			DbConnector target = new DbConnector(connectionString, providerInvariantName);

			Assert.AreEqual(target.ConnectionString, connectionString);
            Assert.AreEqual(target.ProviderFactory.GetType(), typeof(SqlClientFactory));
		}

		/// <summary>
		/// A test for Default
		/// </summary>
		[TestMethod]
		public void DbConnector_Default_get_set_get_set()
		{
			DbConnector previousVal = DbConnector.Default;

			DbConnector val = new DbConnector(ConfigurationManager.ConnectionStrings["Test"]);
			Havit.Data.DbConnector.Default = val;

			Assert.AreEqual(val, Havit.Data.DbConnector.Default, "Havit.Data.DbConnector.Default was not set correctly.");

			DbConnector.Default = previousVal;

			Assert.AreEqual(previousVal, DbConnector.Default);
		}

		/// <summary>
		/// A test for Default
		/// </summary>
		[TestMethod]
		public void DbConnector_Default_get()
		{
			Assert.AreEqual(DbConnector.Default.ConnectionString, ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
			Assert.AreEqual(DbConnector.Default.ProviderFactory.GetType(), typeof(SqlClientFactory));
		}

		/// <summary>
		/// A test for ExecuteScalar (string)
		/// </summary>
		[TestMethod]
		public void DbConnector_ExecuteScalar()
		{
			string commandText = "SELECT Symbol FROM dbo.Role WHERE RoleID=-1";

			object expected = "ZaporneID";
			object actual;

			actual = DbConnector.Default.ExecuteScalar(commandText);

			Assert.AreEqual(expected, actual, "Havit.Data.DbConnector.ExecuteScalar did not return the expected value.");
		}

		[TestMethod]
		public void DbConnector_ExecuteDataSet()
		{
			string commandText = "SELECT * FROM dbo.Role";

			DataSet dataSet = DbConnector.Default.ExecuteDataSet(commandText);

			Assert.IsNotNull(dataSet);
			Assert.AreEqual(1, dataSet.Tables.Count);
		}

		[TestMethod]
		public void DbConnector_ExecuteDataTable()
		{
			string commandText = "SELECT Symbol FROM dbo.Role";

			DataTable dataTable = DbConnector.Default.ExecuteDataTable(commandText);

			Assert.IsNotNull(dataTable);
			Assert.AreEqual(1, dataTable.Columns.Count);
		}

		[TestMethod]
		public void DbConnector_ExecuteNonQuery()
		{
			string commandText = "UPDATE dbo.Role SET Symbol='X' WHERE 0=1";

			int rowsAffected = DbConnector.Default.ExecuteNonQuery(commandText);
			Assert.AreEqual(0, rowsAffected);
		}

		[TestMethod]
		public void DbConnector_ExecuteReader()
		{
			string commandText = "SELECT Symbol FROM dbo.Role";

			DbDataReader reader = DbConnector.Default.ExecuteReader(commandText);
			while (reader.Read())
			{
				string symbol = (string)reader["Symbol"];
			}
		}

		[TestMethod]
		public void DbConnector_ExecuteDataRecord()
		{
			string commandText = "SELECT Symbol FROM dbo.Role WHERE RoleID=-1";

			DataRecord record = DbConnector.Default.ExecuteDataRecord(commandText);

			Assert.IsNotNull(record);
			string symbol = record.Get<string>("Symbol");
		}
	}
}
