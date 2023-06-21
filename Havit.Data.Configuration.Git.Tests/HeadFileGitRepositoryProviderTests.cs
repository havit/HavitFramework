using Havit.Data.Configuration.Git.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Configuration.Git.Tests;

    [TestClass]
    public class HeadFileGitRepositoryProviderTests
    {
        [TestMethod]
        public void HeadFileGitRepositoryProvider_ParseBranchName_MasterBranchRef_MasterIsReturned()
        {
            var headRef = "ref: refs/heads/master";

            string branchName = HeadFileGitRepositoryProvider.ParseBranchName(headRef);

            Assert.AreEqual("master", branchName);
        }

        /// <summary>
        /// Currently parsing branch name from detached head is not supported, returns null.
        /// </summary>
        [TestMethod]
        public void HeadFileGitRepositoryProvider_ParseBranchName_DetachedHead_Null()
        {
            var headRef = "2e479a1f84ad3d6cfd4f5f383a7bb60efada3ccb";

            string branchName = HeadFileGitRepositoryProvider.ParseBranchName(headRef);

            Assert.IsNull(branchName);
        }

        [TestMethod]
        public void HeadFileGitRepositoryProvider_ParseBranchName_FeatureBranchRef_FeatureIsReturned()
        {
            var headRef = "ref: refs/heads/feature/my-super-cool-feature";

            string branchName = HeadFileGitRepositoryProvider.ParseBranchName(headRef);

            Assert.AreEqual("feature/my-super-cool-feature", branchName);
        }
    }