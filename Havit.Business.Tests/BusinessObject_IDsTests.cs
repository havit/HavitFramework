using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.BusinessLayerTest;

namespace Havit.Business.Tests;

[TestClass]
public class BusinessObject_IDsTests
{
	[TestMethod]
	public void BusinessObject_GetObject_SupportsNegativeID()
	{
		using (new IdentityMapScope())
		{
			Role role = Role.GetObject(-1);

			Assert.IsFalse(String.IsNullOrEmpty(role.Symbol));
			Assert.AreEqual(role.Symbol, Role.ZaporneID.Symbol);
		}
	}

	[TestMethod]
	public void BusinessObject_GetObject_SupportsZeroID()
	{
		using (new IdentityMapScope())
		{
			Role role = Role.GetObject(0);

			Assert.IsFalse(String.IsNullOrEmpty(role.Symbol));
			Assert.AreEqual(role.Symbol, Role.NuloveID.Symbol);
		}
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void BusinessObject_GetObject_ThrowsExceptionForNoID()
	{
		using (new IdentityMapScope())
		{
			Role role = Role.GetObject(Role.NoID);
		}
	}

}
