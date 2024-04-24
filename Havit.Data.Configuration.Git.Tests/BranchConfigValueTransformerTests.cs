using System;
using Havit.Data.Configuration.Git.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Configuration.Git.Tests;

[TestClass]
public class BranchConfigValueTransformerTests
{
	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_MasterBranch()
	{
		// Arrange		
		var transformer = new BranchConfigValueTransformer();

		// Act + Assert
		Assert.AreEqual("master", transformer.TransformConfigValue("#BRANCH_NAME#", "master"));
		Assert.AreEqual("_master", transformer.TransformConfigValue("#_BRANCH_NAME#", "master"));
		Assert.AreEqual("-master", transformer.TransformConfigValue("#-BRANCH_NAME#", "master"));
		Assert.AreEqual("master", transformer.TransformConfigValue("{BRANCH_NAME}", "master"));
		Assert.AreEqual("_master", transformer.TransformConfigValue("{_BRANCH_NAME}", "master"));
		Assert.AreEqual("-master", transformer.TransformConfigValue("{-BRANCH_NAME}", "master"));

	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_FeatureBranch()
	{
		// Arrange		
		var transformer = new BranchConfigValueTransformer();

		// Act + Assert
		Assert.AreEqual("feature_test", transformer.TransformConfigValue("#BRANCH_NAME#", "feature/test"));
		Assert.AreEqual("_feature_test", transformer.TransformConfigValue("#_BRANCH_NAME#", "feature/test"));
		Assert.AreEqual("-feature_test", transformer.TransformConfigValue("#-BRANCH_NAME#", "feature/test"));

		Assert.AreEqual("feature_test", transformer.TransformConfigValue("{BRANCH_NAME}", "feature/test"));
		Assert.AreEqual("_feature_test", transformer.TransformConfigValue("{_BRANCH_NAME}", "feature/test"));
		Assert.AreEqual("-feature_test", transformer.TransformConfigValue("{-BRANCH_NAME}", "feature/test"));
	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_UnknownBranch()
	{
		// Arrange		
		var transformer = new BranchConfigValueTransformer();

		// Act + Assert
		Assert.AreEqual("#BRANCH_NAME#", transformer.TransformConfigValue("#BRANCH_NAME#", null));
		Assert.AreEqual("#_BRANCH_NAME#", transformer.TransformConfigValue("#_BRANCH_NAME#", null));
		Assert.AreEqual("#-BRANCH_NAME#", transformer.TransformConfigValue("#-BRANCH_NAME#", null));

		Assert.AreEqual("{BRANCH_NAME}", transformer.TransformConfigValue("{BRANCH_NAME}", null));
		Assert.AreEqual("{_BRANCH_NAME}", transformer.TransformConfigValue("{_BRANCH_NAME}", null));
		Assert.AreEqual("{-BRANCH_NAME}", transformer.TransformConfigValue("{-BRANCH_NAME}", null));


		Assert.AreEqual("#BRANCH_NAME#", transformer.TransformConfigValue("#BRANCH_NAME#", String.Empty));
		Assert.AreEqual("#_BRANCH_NAME#", transformer.TransformConfigValue("#_BRANCH_NAME#", String.Empty));
		Assert.AreEqual("#-BRANCH_NAME#", transformer.TransformConfigValue("#-BRANCH_NAME#", String.Empty));

		Assert.AreEqual("{BRANCH_NAME}", transformer.TransformConfigValue("{BRANCH_NAME}", String.Empty));
		Assert.AreEqual("{_BRANCH_NAME}", transformer.TransformConfigValue("{_BRANCH_NAME}", String.Empty));
		Assert.AreEqual("{-BRANCH_NAME}", transformer.TransformConfigValue("{-BRANCH_NAME}", String.Empty));


	}

	[TestMethod]
	public void BranchConnectionStringConfigurationTransformer_TransformConnectionString_ReplacesMultipleMarkers()
	{
		// Arrange
		var transformer = new BranchConfigValueTransformer();

		// Act + Assert
		Assert.AreEqual("master _master -master master _master -master", transformer.TransformConfigValue("#BRANCH_NAME# #_BRANCH_NAME# #-BRANCH_NAME# {BRANCH_NAME} {_BRANCH_NAME} {-BRANCH_NAME}", "master"));
	}
}