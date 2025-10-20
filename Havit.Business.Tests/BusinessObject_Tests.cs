using Havit.BusinessLayerTest;

namespace Havit.Business.Tests;

[TestClass]
public class BusinessObject_Tests
{
	/// <summary>
	/// Testuje, zda při opakovaných voláních GetAll na cachovaném objektu vrací metoda různé instance kolekce.
	/// </summary>
	[TestMethod]
	public void BusinessObject_GetAll_DoesNotShareCollection()
	{
		using (new IdentityMapScope())
		{
			RoleCollection roleCollection1 = Role.GetAll();
			RoleCollection roleCollection2 = Role.GetAll();
			Assert.AreNotEqual(roleCollection2, roleCollection1);
		}
	}

	[TestMethod]
	public void BusinessObjectBase_GetNullableID_ReturnsIDOfObject()
	{
		using (new IdentityMapScope())
		{
			BusinessObjectBase businessObject = Role.ZaporneID;
			int? expected = Role.ZaporneID.ID;
			int? actual;
			actual = BusinessObjectBase.GetNullableID(businessObject);
			Assert.AreEqual(expected, actual);
		}

	}

	[TestMethod]

	public void BusinessObjectBase_GetNullableID_ReturnsNullForNull()
	{
		BusinessObjectBase businessObject = null;
		int? expected = null;
		int? actual;
		actual = BusinessObjectBase.GetNullableID(businessObject);
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void BusinessObjectBase_FastIntParse_ParsesStringToInt()
	{
		Assert.AreEqual(0, BusinessObjectBase.FastIntParse("0"));
		Assert.AreEqual(1, BusinessObjectBase.FastIntParse("1"));
		Assert.AreEqual(-1, BusinessObjectBase.FastIntParse("-1"));
		Assert.AreEqual(999999, BusinessObjectBase.FastIntParse("999999"));
		Assert.AreEqual(-999999, BusinessObjectBase.FastIntParse("-999999"));
		Assert.AreEqual(123456789, BusinessObjectBase.FastIntParse("123456789"));
	}
}
