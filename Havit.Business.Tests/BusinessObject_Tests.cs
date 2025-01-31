using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Assert.IsTrue(roleCollection1 != roleCollection2);
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
		Assert.AreEqual(BusinessObjectBase.FastIntParse("0"), 0);
		Assert.AreEqual(BusinessObjectBase.FastIntParse("1"), 1);
		Assert.AreEqual(BusinessObjectBase.FastIntParse("-1"), -1);
		Assert.AreEqual(BusinessObjectBase.FastIntParse("999999"), 999999);
		Assert.AreEqual(BusinessObjectBase.FastIntParse("-999999"), -999999);
		Assert.AreEqual(BusinessObjectBase.FastIntParse("123456789"), 123456789);
	}
}
