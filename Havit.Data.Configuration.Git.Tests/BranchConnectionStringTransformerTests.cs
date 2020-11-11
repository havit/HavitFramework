using Havit.Data.Configuration.Git.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Configuration.Git.Tests
{
    [TestClass]
    public class BranchConnectionStringTransformerTests
    {
        [TestMethod]
        public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_ConnectionStringDoesNotContainPlaceholder_ConnectionStringIsUnchanged()
        {
            var transformer = CreateTransformer(currentBranchName: "master");

            var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName;Application Name=MyName";

            string connString = transformer.ChangeDatabaseName(originalConnectionString, "");

            Assert.AreEqual(originalConnectionString, connString);
        }

        [TestMethod]
        public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_BranchIsMaster_ConnectionStringContainsMasterSuffix()
        {
            var transformer = CreateTransformer(currentBranchName: "master");

            string connString = transformer.ChangeDatabaseName("Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName", "");

            Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_master;Application Name=MyName", connString);
        }

        [TestMethod]
        public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_UnknownBranchNull_ConnectionStringIsUnchanged()
        {
            var transformer = CreateTransformer(currentBranchName: null);

            var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName";

            string connString = transformer.ChangeDatabaseName(originalConnectionString, "");

            Assert.AreEqual(originalConnectionString, connString);
        }

        [TestMethod]
        public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_UnknownBranchEmptyString_ConnectionStringIsUnchanged()
        {
            var transformer = CreateTransformer(currentBranchName: "");

            var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName";

            string connString = transformer.ChangeDatabaseName(originalConnectionString, "");

            Assert.AreEqual(originalConnectionString, connString);
        }

        [TestMethod]
        public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_BranchIsTest_ConnectionStringContainsNewDbName()
        {
            var transformer = CreateTransformer(currentBranchName: "test");

            string connString = transformer.ChangeDatabaseName("Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName", "");

            Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_test;Application Name=MyName", connString);
        }

        private static BranchConnectionStringTransformer CreateTransformer(string currentBranchName)
        {
            return new BranchConnectionStringTransformer(Mock.Of<IGitRepositoryProvider>(f => f.GetBranch(It.IsAny<string>()) == currentBranchName));
        }
    }
}