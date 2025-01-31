using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.Tests;

[TestClass]
public class IdentityMapTests
{
	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void IdentityMap_Store()
	{
		IdentityMap target = new IdentityMap();

		TestingBusinessObject businessObject1 = new TestingBusinessObject(10);
		TestingBusinessObject businessObject2 = new TestingBusinessObject(10);

		target.Store(businessObject1);
		target.Store(businessObject2);
	}

	[TestMethod]
	public void IdentityMap_Get_ReturnsStoredObject()
	{
		IdentityMap target = new IdentityMap();

		TestingBusinessObject businessObject = new TestingBusinessObject(10);
		TestingBusinessObject actual;

		target.Store(businessObject);
		actual = target.Get<TestingBusinessObject>(10);

		Assert.AreSame(actual, businessObject);
	}

	[TestMethod]
	public void IdentityMap_Get_ReturnsNullForMissingObject()
	{
		IdentityMap target = new IdentityMap();
		TestingBusinessObject actual = target.Get<TestingBusinessObject>(10);

		Assert.IsNull(actual);
	}

	[TestMethod]
	public void IdentityMap_TryGet_ReturnsTrueForStoredObject()
	{
		IdentityMap im = new IdentityMap();

		TestingBusinessObject businessObject = new TestingBusinessObject(10);
		TestingBusinessObject target;

		im.Store(businessObject);
		bool result = im.TryGet<TestingBusinessObject>(10, out target);

		Assert.AreSame(target, businessObject);
		Assert.IsTrue(result);
	}

	[TestMethod]
	public void IdentityMap_TryGet_ReturnsFalseForMissingObject()
	{
		IdentityMap im = new IdentityMap();

		TestingBusinessObject businessObject = new TestingBusinessObject(10);
		TestingBusinessObject target;

		im.Store(businessObject);
		bool result = im.TryGet<TestingBusinessObject>(11, out target);

		Assert.IsNull(target);
		Assert.IsFalse(result);
	}

	[TestMethod]
	public void IdentityMap_UseOnlyWeakReferenceToAllowObjectToBeCollectedByGC()
	{
#if DEBUG
		Assert.Inconclusive("Cannot run this unit test in debug mode.");
#endif

		IdentityMap im = new IdentityMap();

		TestingBusinessObject businessObject = new TestingBusinessObject(10);
		TestingBusinessObject target;

		im.Store(businessObject);
		businessObject = null; // potřeba pro možnost uvolnění GC
		GC.Collect();
		bool result = im.TryGet<TestingBusinessObject>(10, out target);

		Assert.IsNull(target);
		Assert.IsFalse(result);
	}

	public class TestingBusinessObject : BusinessObjectBase
	{
		public TestingBusinessObject() : base(ConnectionMode.Connected)
		{
		}

		public TestingBusinessObject(int id)
			: base(id, ConnectionMode.Connected)
		{
		}

		protected override bool TryLoad_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override void Save_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		protected override void Delete_Perform(System.Data.Common.DbTransaction transaction)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
