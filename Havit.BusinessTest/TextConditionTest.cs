using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Havit.Business;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Test TextCondition.
	/// </summary>
	[TestClass]
	public class TextConditionTest
	{
		#region CreateIsNullOrEmptyTest
		[TestMethod]
		public void CreateIsNullOrEmptyTest()
		{
			using (new IdentityMapScope())
			{
				QueryParams qp;

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create("")));
				Assert.IsTrue(Role.GetList(qp).Count > 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create((string)null)));
				Assert.IsTrue(Role.GetList(qp).Count > 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create("aaa")));
				Assert.IsTrue(Role.GetList(qp).Count == 0);
			}
		}
		#endregion

		#region CreateIsNotNullOrEmptyTest
		[TestMethod]
		public void CreateIsNotNullOrEmptyTest()
		{
			using (new IdentityMapScope())
			{
				QueryParams qp;

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create("")));
				Assert.IsTrue(Role.GetList(qp).Count == 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create((string)null)));
				Assert.IsTrue(Role.GetList(qp).Count == 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create("aaa")));
				Assert.IsTrue(Role.GetList(qp).Count > 0);
			}
		}
		#endregion

		#region CreateWildcardsTest
		[TestMethod]
		public void CreateWildcardsTest()
		{
			using (new IdentityMapScope())
			{
				QueryParams qp;

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateWildcards(ValueOperand.Create("["), "["));
				Assert.IsTrue(Role.GetList(qp).Count > 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateWildcards(ValueOperand.Create("]"), "]"));
				Assert.IsTrue(Role.GetList(qp).Count > 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateWildcards(ValueOperand.Create("["), "]"));
				Assert.IsTrue(Role.GetList(qp).Count == 0);

				qp = new QueryParams();
				qp.Conditions.Add(TextCondition.CreateWildcards(ValueOperand.Create("]"), "["));
				Assert.IsTrue(Role.GetList(qp).Count == 0);
			}
		}
		#endregion
	}
}
