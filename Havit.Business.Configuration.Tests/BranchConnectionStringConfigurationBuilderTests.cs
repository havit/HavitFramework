using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Business.Configuration.Tests
{
	[TestClass]
	public class BranchConnectionStringConfigurationBuilderTests
	{
		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_ProcessConfigurationSection_NonConnectionStringSection_ReturnsOriginalSection()
		{
			var builder = CreateBuilder(null);

			var appSettingsSection = new AppSettingsSection();

			ConfigurationSection section = builder.ProcessConfigurationSection(appSettingsSection);

			Assert.AreSame(appSettingsSection, section);
		}

		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_TransformConnectionString_BranchIsMaster_ConnectionStringIsUnchanged()
		{
			var builder = CreateBuilder(currentBranchName: "master");

			var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll;Application Name=AccacePayroll";
			string connString = builder.TransformConnectionString(originalConnectionString);

			Assert.AreEqual(originalConnectionString, connString);
		}


		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_TransformConnectionString_BranchIsTest_ConnectionStringContainsNewDbName()
		{
			var builder = CreateBuilder(currentBranchName: "test");

			string connString = builder.TransformConnectionString("Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll;Application Name=AccacePayroll");

			Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll_test;Application Name=AccacePayroll", connString);
		}

		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_ProcessConfigurationSection_BranchIsTest_ReturnsConnectionStringSection()
		{
			var builder = CreateBuilder(currentBranchName: "test");

			var configurationSection = new ConnectionStringsSection()
			{
				ConnectionStrings = { new ConnectionStringSettings("DefaultConnectionString", "Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll;Application Name=AccacePayroll") }
			};

			ConfigurationSection modifiedSection = builder.ProcessConfigurationSection(configurationSection);
			Assert.IsInstanceOfType(modifiedSection, typeof(ConnectionStringsSection));
		}

		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_ProcessConfigurationSection_BranchIsTest_ConnectionStringContainsNewDbName()
		{
			var builder = CreateBuilder(currentBranchName: "test");

			var configurationSection = new ConnectionStringsSection
			{
				ConnectionStrings = { new ConnectionStringSettings("DefaultConnectionString", "Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll;Application Name=AccacePayroll") }
			};

			var modifiedSection = (ConnectionStringsSection)builder.ProcessConfigurationSection(configurationSection);

			ConnectionStringSettings connString = modifiedSection.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(s => s.Name == "DefaultConnectionString");
			Assert.IsNotNull(connString);
			Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=AccacePayroll_test;Application Name=AccacePayroll", connString.ConnectionString);
		}

		[TestMethod]
		public void BranchConnectionStringConfigurationBuilder_ProcessConfigurationSection_ConnectionStringWithoutInitialCatalog_ReturnsOriginalConnectionString()
		{
			var builder = CreateBuilder(currentBranchName: "test");

			const string OriginalConnectionString = "data source=.\\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename=|DataDirectory|aspnetdb.mdf;User Instance=true";
			var configurationSection = new ConnectionStringsSection()
			{
				ConnectionStrings = { new ConnectionStringSettings("DefaultConnectionString", OriginalConnectionString) }
			};

			var modifiedSection = (ConnectionStringsSection)builder.ProcessConfigurationSection(configurationSection);

			ConnectionStringSettings connString = modifiedSection.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(s => s.Name == "DefaultConnectionString");
			Assert.IsNotNull(connString);
			Assert.AreEqual(OriginalConnectionString, connString.ConnectionString);
		}

		private static BranchConnectionStringConfigurationBuilder CreateBuilder(string currentBranchName)
		{
			return new BranchConnectionStringConfigurationBuilder(Mock.Of<ICurrentGitRepositoryProvider>(f => f.GetCurrentBranch() == currentBranchName));
		}
	}
}
