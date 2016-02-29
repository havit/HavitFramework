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
		[TestMethod]
		public void CompositeCondition_IsEmptyWhenInitializedWithEmptyConditions()
		{
			AndCondition andCondition = new AndCondition(EmptyCondition.Create(), EmptyCondition.Create());
			Assert.IsTrue(andCondition.IsEmptyCondition());
		}

		[TestMethod]
		public void CompositeCondition_IsEmptyAfterAddingEmptyConditions()
		{
			AndCondition andCondition = new AndCondition();
			andCondition.Conditions.Add(EmptyCondition.Create());
			Assert.AreEqual(andCondition.Conditions.Count, 0);
		}

		[TestMethod]
		public void CompositeCondition_IsEmptyAfterAddingEmptyCompositeCondition()
		{
			AndCondition andCondition = new AndCondition(
				new OrCondition(EmptyCondition.Create(), EmptyCondition.Create()),
				EmptyCondition.Create(),
				EmptyCondition.Create());
			Assert.IsTrue(andCondition.IsEmptyCondition());
		}

		[TestMethod]
		public void CompositeCondition_IsNotEmptyAfterAddingNotEmptyCompositeCondition()
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
		public void CompositeCondition_CanCreateSqlQueryAfterAddingNonEmptyCompositeCondition()
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
	}
}
