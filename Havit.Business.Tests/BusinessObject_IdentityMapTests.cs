using Havit.BusinessLayerTest;
using Havit.Business.Query;

namespace Havit.Business.Tests;

[TestClass]
public class BusinessObject_IdentityMapTests
{
	[TestMethod]
	public void BusinessObject_GetFirst_ReturnsExistingObjectFromIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			QueryParams qp = new QueryParams();
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Assert.AreSame(uzivatel1, uzivatel2);
		}
	}

	[TestMethod]
	public void BusinessObject_GetFirst_StoresLoadedObjectToIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			QueryParams qp = new QueryParams();
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			Assert.AreSame(uzivatel1, uzivatel2);
		}
	}

	[TestMethod]
	public void BusinessObject_GetFirst_Ghost_ReturnsExistingObjectFromIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			QueryParams qp = new QueryParams();
			qp.Properties.Add(Uzivatel.Properties.ID);
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Assert.AreSame(uzivatel1, uzivatel2);
		}
	}

	[TestMethod]
	public void BusinessObject_GetFirst_Ghost_StoresLoadedObjectToIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			QueryParams qp = new QueryParams();
			qp.Properties.Add(Uzivatel.Properties.ID);
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			Assert.AreSame(uzivatel1, uzivatel2);
		}
	}

	/// <summary>
	/// Test na IM při partial-load GetFirst.
	/// Pokud je objekt načítán pouze částečně, nepatří do IdentityMap.
	/// </summary>
	[TestMethod]
	public void BusinessObject_GetFirst_PartialLoad_DoesNotReturnObjectFromIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			QueryParams qp = new QueryParams();
			qp.Properties.Add(Uzivatel.Properties.ID);
			qp.Properties.Add(Uzivatel.Properties.Username);
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Assert.AreNotSame(uzivatel1, uzivatel2);
		}
	}

	/// <summary>
	/// Test na IM při partial-load GetFirst.
	/// Pokud je objekt načítán pouze částečně, nepatří do IdentityMap.
	/// </summary>
	[TestMethod]
	public void BusinessObject_GetFirst_PartialLoad_DoesNotStoreObjectToIdentityMap()
	{
		using (IdentityMapScope ims = new IdentityMapScope())
		{
			QueryParams qp = new QueryParams();
			qp.Properties.Add(Uzivatel.Properties.ID);
			qp.Properties.Add(Uzivatel.Properties.Username);
			qp.Conditions.Add(NumberCondition.CreateEquals(Uzivatel.Properties.ID, 1));
			Uzivatel uzivatel2 = Uzivatel.GetFirst(qp);

			Uzivatel uzivatel1 = Uzivatel.GetObject(1);

			Assert.AreNotSame(uzivatel1, uzivatel2);
		}
	}
}
