using System.Diagnostics;
using System.Linq.Expressions;
using Havit.Linq.Expressions;

namespace Havit.Core.Tests.Linq.Expressions;

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
	public void ExpressionExt_SubstituteParameter_NestedLambda()
	{
		// Act
		Expression<Func<A, bool>> expression = ExpressionExt.SubstituteParameter<B, A, bool>(b => b.Items.Any(i => i.X % 2 == 0), a => a.B);

		// Assert
		Func<A, bool> lambda = expression.Compile();
		Assert.IsTrue(lambda.Invoke(new A { B = new B { Items = new List<Item> { new Item { X = 1 }, new Item { X = 2 } } } }));
		Assert.IsFalse(lambda.Invoke(new A { B = new B { Items = new List<Item> { new Item { X = 1 }, new Item { X = 3 } } } }));
	}

	[TestMethod]
	public void ExpressionExt_SubstituteParameter_NestedLambdaUsingOuterParameter()
	{
		// Act
		Expression<Func<A, bool>> expression = ExpressionExt.SubstituteParameter<B, A, bool>(b => b.Items.Any(i => i.X == b.Y), a => a.B);

		// Assert
		Func<A, bool> lambda = expression.Compile();
		Assert.IsTrue(lambda.Invoke(new A { B = new B { Y = 2, Items = new List<Item> { new Item { X = 1 }, new Item { X = 2 } } } }));
		Assert.IsFalse(lambda.Invoke(new A { B = new B { Y = 5, Items = new List<Item> { new Item { X = 1 }, new Item { X = 2 } } } }));
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
		Assert.HasCount(1, result);
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
	public void ExpressionExt_GetMemberAccessMemberName_UnsupportedArgument()
	{
		// Arrange
		Expression<Func<A, bool>> expression = (A a) => a.B.C;

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			ExpressionExt.GetMemberAccessMemberName(expression);
		});
	}

	internal class A
	{
		public B B { get; set; }
	}

	internal class B
	{
		public bool C { get; set; }
		public int Y { get; set; }
		public List<Item> Items { get; set; }
	}

	internal class Item
	{
		public int X { get; set; }
	}

}
