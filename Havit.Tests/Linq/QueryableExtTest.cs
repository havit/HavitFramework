using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Linq
{
	[TestClass]
	public class QueryableExtTest
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
	}
}
