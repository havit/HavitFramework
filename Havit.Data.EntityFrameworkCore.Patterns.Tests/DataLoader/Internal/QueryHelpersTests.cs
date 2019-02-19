using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal
{
	[TestClass]
	public class QueryHelpersTests
	{
		[TestMethod]
		public void QueryHelpers_ContainsEffective_Zero()
		{
			// Act
			var result = GetData().Where(new List<int> { }.ContainsEffective<TestClass>(p => p.Id)).ToList();

			// Assert
			Assert.IsFalse(result.Any());
		}

		[TestMethod]
		public void QueryHelpers_ContainsEffective_One()
		{
			// Act
			var result = GetData().Where(new List<int> { 2 }.ContainsEffective<TestClass>(p => p.Id)).ToList();

			// Assert
			Assert.AreEqual(1, result.Count, "Count");
			Assert.IsTrue(result.Any(item => item.Id == 2), "2");
		}

		[TestMethod]
		public void QueryHelpers_ContainsEffective_Sorted()
		{
			// Act
			var result = GetData().Where(new List<int> { 2, 3, 4 }.ContainsEffective<TestClass>(p => p.Id)).ToList();

			// Assert
			Assert.AreEqual(3, result.Count, "Count");
			Assert.IsTrue(result.Any(item => item.Id == 2), "2");
			Assert.IsTrue(result.Any(item => item.Id == 3), "3");
			Assert.IsTrue(result.Any(item => item.Id == 4), "4");
		}

		[TestMethod]
		public void QueryHelpers_ContainsEffective_NotSorted()
		{
			// Act
			var result = GetData().Where(new List<int> { 1, 2, 3, 5 }.ContainsEffective<TestClass>(p => p.Id)).ToList();

			// Assert
			Assert.AreEqual(4, result.Count, "Count");
			Assert.IsTrue(result.Any(item => item.Id == 1), "1");
			Assert.IsTrue(result.Any(item => item.Id == 2), "2");
			Assert.IsTrue(result.Any(item => item.Id == 3), "3");
			Assert.IsTrue(result.Any(item => item.Id == 5), "5");
		}

		private IQueryable<TestClass> GetData()
		{
			return new List<TestClass>
			{
				new TestClass { Id = 1 },
				new TestClass { Id = 2 },
				new TestClass { Id = 3 },
				new TestClass { Id = 4 },
				new TestClass { Id = 5 },
			}.AsQueryable();
		}

		internal class TestClass
		{
			public int Id { get; set; }
		}
	}
}
