using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Business;
using Havit.BusinessLayerTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.BusinessTest
{
	[TestClass]
	public class BusinessObject_EqualityTests
	{
		[TestMethod]
		public void BusinessObject_Equals_EqualsGetObjectWithSameID()
		{
			using (new IdentityMapScope())
			{
				Role role1 = Role.GetObject(-1);
				Role role2 = Role.GetObject(-1);

				Assert.IsTrue(role1.Equals(role2));
				Assert.IsTrue(role2.Equals(role1));
				Assert.IsTrue(role1 == role2);
				Assert.IsTrue(role2 == role1);
			}
		}

		[TestMethod]
		public void BusinessObject_Equals_DoesNoEqualGetObjectWithDifferentIDs()
		{
			using (new IdentityMapScope())
			{
				Role role1 = Role.GetObject(-1);
				Role role2 = Role.GetObject(1);

				Assert.IsFalse(role1.Equals(role2));
				Assert.IsFalse(role2.Equals(role1));
				Assert.IsFalse(role1 == role2);
				Assert.IsFalse(role2 == role1);
			}
		}

		[TestMethod]
		public void BusinessObject_Equals_DoesNoEqualNewObjects()
		{
			Subjekt subjekt1 = Subjekt.CreateObject();
			Subjekt subjekt2 = Subjekt.CreateObject();
			Subjekt subjekt3 = subjekt1;

			Assert.IsFalse(subjekt1.Equals(subjekt2));
			Assert.IsFalse(subjekt1.Equals(subjekt2));
			Assert.IsFalse(subjekt1 == subjekt2);
			Assert.IsFalse(subjekt1 == subjekt2);

			Assert.IsTrue(subjekt1.Equals(subjekt3));
			Assert.IsTrue(subjekt1.Equals(subjekt3));
			Assert.IsTrue(subjekt1 == subjekt3);
			Assert.IsTrue(subjekt1 == subjekt3);
		}
	}
}
