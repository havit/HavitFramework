using System.Configuration;
using System.Linq;
using Havit.Data.Configuration.Git;
using Havit.Data.Configuration.Git.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Configuration.Git.Tests
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
		public void BranchConnectionStringConfigurationBuilder_ProcessConfigurationSection_BranchIsTest_ReturnsConnectionStringSection()
		{
			var builder = CreateBuilder(currentBranchName: "test");

			var configurationSection = new ConnectionStringsSection()
			{
				ConnectionStrings = { new ConnectionStringSettings("DefaultConnectionString", "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName") }
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
				ConnectionStrings = { new ConnectionStringSettings("DefaultConnectionString", "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName") }
			};

			var modifiedSection = (ConnectionStringsSection)builder.ProcessConfigurationSection(configurationSection);

			ConnectionStringSettings connString = modifiedSection.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(s => s.Name == "DefaultConnectionString");
			Assert.IsNotNull(connString);
			Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_test;Application Name=MyName", connString.ConnectionString);
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
			return new BranchConnectionStringConfigurationBuilder(Mock.Of<IGitRepositoryProvider>(f => f.GetBranch(It.IsAny<string>()) == currentBranchName));
		}
	}
}
