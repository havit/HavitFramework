using Havit.BusinessLayerTest;
using Havit.BusinessLayerTest.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Business.Tests
{
    [TestClass]
    public class DbResourcesTests
    {
        [TestMethod]
        public void DbResources_MainResourcesClass_MainResourceKey()
        {
            // Act + Assert
            using (new IdentityMapScope())
            {
                Assert.AreEqual("CzechValue", new DbResources(Language.Czech).MainResourceClass.MainResourceKey);
                Assert.AreEqual("EnglishValue", new DbResources(Language.English).MainResourceClass.MainResourceKey);
            }
        }

        [TestMethod]
        public void DbResources_MainResourcesClass_MainResourceKey_UsesFallbackLanguage()
        {
            // No value is defined for spanish, using fallback to english value

            // Act + Assert
            using (new IdentityMapScope())
            {
                Assert.AreEqual("EnglishValue", new DbResources(Language.Spanish).MainResourceClass.MainResourceKey);
            }
        }
    }
}
