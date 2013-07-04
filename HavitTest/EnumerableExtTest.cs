using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace HavitTest
{
	[TestClass]
	public class EnumerableExtTest
	{
		#region LeftJoinTest
		[TestMethod]
		public void LeftJoinTest()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.LeftJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.LeftJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(joinedData1.Count, 14);
			Assert.AreEqual(joinedData2.Count, 14);
		}
		#endregion
		
		#region RightJoinTest
		[TestMethod]
		public void RightJoinTest()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.RightJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.RightJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(joinedData1.Count, 16);
			Assert.AreEqual(joinedData2.Count, 16);
		}
		#endregion

		#region FullJoinTest
		[TestMethod]
		public void FullJoinTest()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.FullOuterJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.FullOuterJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(joinedData1.Count, 22); // 5ka je v obou kolekcích
			Assert.AreEqual(joinedData2.Count, 22); // 5ka je v obou kolekcích
		}
		#endregion

	}
}
