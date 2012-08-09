using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Summary description for TextConditionTest
	/// </summary>
	[TestClass]
	public class TextConditionTest
	{
		[TestMethod]
		public void CreateIsNullOrEmptyTest()
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

		[TestMethod]
		public void CreateIsNotNullOrEmptyTest()
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
}
