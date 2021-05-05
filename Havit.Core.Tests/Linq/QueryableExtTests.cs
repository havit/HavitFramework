using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Havit.Collections;
using Havit.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq
{
	[TestClass]
	public class QueryableExtTests
	{
		[TestMethod]
		public void QueryableExt_WhereIf()
		{
			// Arrange
			IQueryable<int> numbers = (new[] { 0 }).AsQueryable();

			// Act
			List<int> result1 = numbers.WhereIf(true, i => i > 0).ToList();
			List<int> result2 = numbers.WhereIf(false, i => i > 0).ToList();

			// Assert
			Assert.AreEqual(0, result1.Count);
			Assert.AreEqual(1, result2.Count);
		}

		[TestMethod]
		public void QueryableExt_OrderBy_WithMappingFunction()
		{
			// Arrange
			IQueryable<int> numbers = (new[] { 2, 3, 1 }).AsQueryable();

			// second item in sortItems should have no impact (no duplicates in integer array & already sorted by "A")
			SortItem[] sortItems1 = new[] { new SortItem { Expression = "A", Direction = SortDirection.Ascending }, new SortItem { Expression = "A", Direction = SortDirection.Descending } }; // use nameof(...) instead of string literal
			SortItem[] sortItems2 = new[] { new SortItem { Expression = "A", Direction = SortDirection.Descending }, new SortItem { Expression = "A", Direction = SortDirection.Ascending } }; // use nameof(...) instead of string literal

			// Act
			List<int> result1 = numbers.OrderBy(sortItems1, sortExpression => sortExpression switch
				{
					"A" => i => i, // use nameof(...) instead of string literal
					_ => throw new InvalidOperationException(sortExpression)
				})
				.ToList();

			List<int> result2 = numbers.OrderBy(sortItems2, sortExpression => sortExpression switch
			{
				"A" => i => i, // use nameof(...) instead of string literal
				_ => throw new InvalidOperationException(sortExpression)
			})
				.ToList();


			// Assert
			Assert.AreEqual(1, result1[0]);
			Assert.AreEqual(2, result1[1]);
			Assert.AreEqual(3, result1[2]);

			Assert.AreEqual(3, result2[0]);
			Assert.AreEqual(2, result2[1]);
			Assert.AreEqual(1, result2[2]);
		}

		[TestMethod]
		public void QueryableExt_OrderBy_WithMappingFunction_NullSortItems()
		{
			// Arrange
			IQueryable<int> numbers = (new[] { 2, 3, 1 }).AsQueryable();

			// Act
			List<int> result = numbers.OrderBy(null, (Func<string, Expression<Func<int, object>>>)(sortExpression => throw new InvalidOperationException(sortExpression))).ToList();

			// Assert (order is not changed)
			CollectionAssert.AreEqual(numbers.ToList(), result);
		}

		[TestMethod]
		public void QueryableExt_OrderBy_WithMappingFunctionForMultipleExpressions()
		{
			// Arrange
			IQueryable<int> numbers = (new[] { 2, 3, 1 }).AsQueryable();

			// second item in sortItems should have no impact (no duplicates in integer array & already sorted by "A")
			SortItem[] sortItems1 = new[] { new SortItem { Expression = "A", Direction = SortDirection.Ascending }, new SortItem { Expression = "A", Direction = SortDirection.Descending } }; // use nameof(...) instead of string literal
			SortItem[] sortItems2 = new[] { new SortItem { Expression = "A", Direction = SortDirection.Descending }, new SortItem { Expression = "A", Direction = SortDirection.Ascending } }; // use nameof(...) instead of string literal

			// Act
			List<int> result1 = numbers.OrderBy(sortItems1, QueryableExt_OrderBy_WithMappingFunctionForMultipleExpressions_MappingFunction).ToList();
			List<int> result2 = numbers.OrderBy(sortItems2, QueryableExt_OrderBy_WithMappingFunctionForMultipleExpressions_MappingFunction).ToList();

			// Assert
			Assert.AreEqual(1, result1[0]);
			Assert.AreEqual(2, result1[1]);
			Assert.AreEqual(3, result1[2]);

			Assert.AreEqual(3, result2[0]);
			Assert.AreEqual(2, result2[1]);
			Assert.AreEqual(1, result2[2]);
		}

		[TestMethod]
		public void QueryableExt_OrderBy_WithMappingFunctionForMultipleExpressions_NullSortItems()
		{
			// Arrange
			IQueryable<int> numbers = (new[] { 2, 3, 1 }).AsQueryable();

			// Act
			List<int> result = numbers.OrderBy(null, (Func<string, IEnumerable<Expression<Func<int, object>>>>)(sortExpression => throw new InvalidOperationException(sortExpression))).ToList();

			// Assert (order is not changed)
			CollectionAssert.AreEqual(numbers.ToList(), result);
		}

		private IEnumerable<Expression<Func<int, object>>> QueryableExt_OrderBy_WithMappingFunctionForMultipleExpressions_MappingFunction(string sortExpression)
		{
			// we need to extract the method here because yield return statement is not allowed in anonymous methods (&lambdas)
			switch (sortExpression)
			{
				case /*nameof(...)*/ "A":
					yield return i => i;
					yield return i => i; // example how to yield multiple sort expressions for one sortExpression
					yield break;

				default: throw new InvalidOperationException(sortExpression);
			}
		}

	}
}
