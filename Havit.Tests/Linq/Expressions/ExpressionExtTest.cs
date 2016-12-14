using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Havit.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq.Expressions
{
	[TestClass]
	public class ExpressionExtTest
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
