using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Havit.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq.Expressions
{
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

		internal class A
		{
			public B B { get; set; }
		}

		internal class B
		{
			public bool C { get; set; }
		}

	}
}
