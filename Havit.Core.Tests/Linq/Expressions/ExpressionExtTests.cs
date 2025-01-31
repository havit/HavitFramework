﻿using System.Diagnostics;
using System.Linq.Expressions;
using Havit.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq.Expressions;

[TestClass]
public class ExpressionExtTests
{
	[TestMethod]
	public void ExpressionExt_SubstituteParameter()
	{
		// Act
		Expression<Func<A, bool>> expression = ExpressionExt.SubstituteParameter<B, A, bool>(b => b.C, a => a.B);

		// Assert
		Assert.IsTrue(expression.Compile().Invoke(new A { B = new B() { C = true } }));
		Assert.IsFalse(expression.Compile().Invoke(new A { B = new B() { C = false } }));
	}

	[TestMethod]
	public void ExpressionExt_AndAlso()
	{
		// Arrange
		Expression<Func<A, bool>> condition = (A a) => a.B != null;
		condition = ExpressionExt.AndAlso(null, condition, null, item => item.B.C);

		Debug.WriteLine(condition);

		Func<A, bool> conditionLambda = condition.Compile();

		List<A> list = new List<A>
		{
			new A(), // není B
			new A { B = new B { C = false } }, // C je false
			new A { B = new B { C = true } } // C je true, tento záznam chceme najít
		};

		// Act
		List<A> result = list.Where(conditionLambda).ToList();

		// Assert
		Assert.AreEqual(1, result.Count);
		Assert.AreSame(list[2], result.Single());
	}

	[TestMethod]
	public void ExpressionExt_GetMemberAccessMemberName()
	{
		// Act
		Expression<Func<B, bool>> expression = (B b) => b.C;
		string result = ExpressionExt.GetMemberAccessMemberName(expression);

		// Assert
		Assert.AreEqual("C", result);
	}

	[TestMethod]
	public void ExpressionExt_GetMemberAccessMemberName_SupportsConvert()
	{
		// Act
		Expression<Func<B, object>> expression = (B b) => b.C; // použití typu "object" je v tomto testu klíčové.
		string result = ExpressionExt.GetMemberAccessMemberName(expression);

		// Assert
		Assert.AreEqual("C", result);
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void ExpressionExt_GetMemberAccessMemberName_UnsupportedArgument()
	{
		// Act
		Expression<Func<A, bool>> expression = (A a) => a.B.C;
		string result = ExpressionExt.GetMemberAccessMemberName(expression);

		// Assert by method attribute
	}

	internal class A
	{
		public B B { get; set; }
	}

	internal class B
	{
		public bool C { get; set; }
	}

}
