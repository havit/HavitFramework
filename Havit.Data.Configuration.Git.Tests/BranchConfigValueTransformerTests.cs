using Havit.Data.Configuration.Git.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Configuration.Git.Tests;

[TestClass]
public class BranchConfigValueTransformerTests
{
	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_ConnectionStringDoesNotContainPlaceholder_ConnectionStringIsUnchanged()
	{
		var transformer = CreateTransformer();

		var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName;Application Name=MyName";

		string connString = transformer.TransformConfigValue(originalConnectionString, "master");

		Assert.AreEqual(originalConnectionString, connString);
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_BranchIsMaster_ConnectionStringContainsMasterSuffix()
	{
		var transformer = CreateTransformer();

		string connString = transformer.TransformConfigValue("Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName", "master");

		Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_master;Application Name=MyName", connString);
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_UnknownBranchNull_ConnectionStringIsUnchanged()
	{
		var transformer = CreateTransformer();

		var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName";

		string connString = transformer.TransformConfigValue(originalConnectionString, null);

		Assert.AreEqual(originalConnectionString, connString);
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_UnknownBranchEmptyString_ConnectionStringIsUnchanged()
	{
		var transformer = CreateTransformer();

		var originalConnectionString = "Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName";

		string connString = transformer.TransformConfigValue(originalConnectionString, "");

		Assert.AreEqual(originalConnectionString, connString);
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_BranchIsTest_ConnectionStringContainsNewDbName()
	{
		var transformer = CreateTransformer();

		string connString = transformer.TransformConfigValue("Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName", "test");

		Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_test;Application Name=MyName", connString);
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_BranchIsFeature_ConnectionStringContainsNewDbName()
	{
		var transformer = CreateTransformer();

		string connString = transformer.TransformConfigValue("Data Source=(localdb)\v14.0;Initial Catalog=MyName_#BRANCH_NAME#;Application Name=MyName", "feature/test");

		Assert.AreEqual("Data Source=(localdb)\v14.0;Initial Catalog=MyName_feature_test;Application Name=MyName", connString);
	}

	private static IBranchConfigValueTransformer CreateTransformer()
	{
		return new BranchConfigValueTransformer();
	}
}