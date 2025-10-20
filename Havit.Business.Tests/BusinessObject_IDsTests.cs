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
	public void BusinessObject_GetObject_ThrowsExceptionForNoID()
	{
		// Arrange
		using (new IdentityMapScope())
		{
			// Assert
			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				// Act
				_ = Role.GetObject(Role.NoID);
			});
		}
	}

}
