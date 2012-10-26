using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Summary description for CompositeConditionTest
	/// </summary>
	[TestClass]
	public class CompositeConditionTest
	{
		[TestMethod]
		public void AndCondition_ConstructorWithEmptyConditionsTest()
		{
			AndCondition andCondition = new AndCondition(EmptyCondition.Create(), EmptyCondition.Create());
			Assert.AreEqual(andCondition.Conditions.Count, 0);
		}

		[TestMethod]
		public void AndCondition_AddEmptyConditionsTest()
		{
			AndCondition andCondition = new AndCondition();
			andCondition.Conditions.Add(EmptyCondition.Create());
			Assert.AreEqual(andCondition.Conditions.Count, 0);
		}

		[TestMethod]
		public void CompositeCondition_IsEmptyTest()
		{
			AndCondition andCondition = new AndCondition(
				new OrCondition(EmptyCondition.Create(), EmptyCondition.Create()),
				EmptyCondition.Create(),
				EmptyCondition.Create());
			Assert.IsTrue(andCondition.IsEmptyCondition());
		}

		[TestMethod]
		public void CompositeCondition_IsNotEmptyTest()
		{
			AndCondition andCondition = new AndCondition(
				new OrCondition(
					EmptyCondition.Create(),
					EmptyCondition.Create(),
					BoolCondition.CreateTrue(Uzivatel.Properties.Disabled)),
				EmptyCondition.Create(),
				EmptyCondition.Create());
			Assert.IsFalse(andCondition.IsEmptyCondition());
		}

		[TestMethod]
		public void CompositeCondition_NestedConditionTest()
		{
			QueryParams queryParams = new QueryParams();
			queryParams.Conditions.Add(
				new AndCondition(
					new OrCondition(
						new AndCondition(
							EmptyCondition.Create(),
							EmptyCondition.Create()))));
			Uzivatel.GetList(queryParams);
		}
	}
}
