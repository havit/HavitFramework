using System;
using System.Text;
using System.Collections.Generic;

using Havit.Business;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Test kompozitních podmínek.
	/// </summary>
	[TestClass]
	public class CompositeConditionTest
	{
		#region AndCondition_ConstructorWithEmptyConditionsTest
		[TestMethod]
		public void AndCondition_ConstructorWithEmptyConditionsTest()
		{
			AndCondition andCondition = new AndCondition(EmptyCondition.Create(), EmptyCondition.Create());
			Assert.AreEqual(andCondition.Conditions.Count, 0);
		}
		#endregion

		#region AndCondition_AddEmptyConditionsTest
		[TestMethod]
		public void AndCondition_AddEmptyConditionsTest()
		{
			AndCondition andCondition = new AndCondition();
			andCondition.Conditions.Add(EmptyCondition.Create());
			Assert.AreEqual(andCondition.Conditions.Count, 0);
		}
		#endregion

		#region CompositeCondition_IsEmptyTest
		[TestMethod]
		public void CompositeCondition_IsEmptyTest()
		{
			AndCondition andCondition = new AndCondition(
				new OrCondition(EmptyCondition.Create(), EmptyCondition.Create()),
				EmptyCondition.Create(),
				EmptyCondition.Create());
			Assert.IsTrue(andCondition.IsEmptyCondition());
		}
		#endregion

		#region CompositeCondition_IsNotEmptyTest
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
		#endregion

		#region CompositeCondition_NestedConditionTest
		[TestMethod]
		public void CompositeCondition_NestedConditionTest()
		{
			using (new IdentityMapScope())
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
		#endregion
	}
}
