using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Havit.Business;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;
using Havit.BusinessLayerTest;

namespace Havit.Business.Tests;

/// <summary>
/// Test TextCondition.
/// </summary>
[TestClass]
public class TextConditionTests
{
	[TestMethod]
	public void TextCondition_CreateIsNullOrEmpty_OnlyNullOrEmptyTextsAreReturned()
	{
		using (new IdentityMapScope())
		{
			QueryParams qp;

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create("")));
			Assert.IsTrue(Role.GetList(qp).Count > 0);

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create(null)));
			Assert.IsTrue(Role.GetList(qp).Count > 0);

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNullOrEmpty(ValueOperand.Create("aaa")));
			Assert.IsTrue(Role.GetList(qp).Count == 0);
		}
	}

	[TestMethod]
	public void TextCondition_CreateIsNotNullOrEmpty_OnlyNotNullAndNotEmptyTextsAreReturned()
	{
		using (new IdentityMapScope())
		{
			QueryParams qp;

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create("")));
			Assert.IsTrue(Role.GetList(qp).Count == 0);

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create(null)));
			Assert.IsTrue(Role.GetList(qp).Count == 0);

			qp = new QueryParams();
			qp.Conditions.Add(TextCondition.CreateIsNotNullOrEmpty(ValueOperand.Create("aaa")));
			Assert.IsTrue(Role.GetList(qp).Count > 0);
		}
	}

	[TestMethod]
	public void TextCondition_CreateWildcards_HandlesWellSquareBrackets()
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
}
