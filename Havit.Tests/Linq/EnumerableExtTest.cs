using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Tests.Linq
{
	[TestClass]
	public class EnumerableExtTest
	{
		[TestMethod]
		public void EnumerableExt_LeftJoin()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.LeftJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.LeftJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(14, joinedData1.Count);
			Assert.AreEqual(14, joinedData2.Count);
		}
		
		[TestMethod]
		public void EnumerableExt_RightJoin()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.RightJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.RightJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(16, joinedData1.Count);
			Assert.AreEqual(16, joinedData2.Count);
		}

		[TestMethod]
		public void EnumerableExt_FullJoin()
		{
			IEnumerable<int> data1 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)); // 1..5, 1..5
			IEnumerable<int> data2 = Enumerable.Range(4, 6).Concat(Enumerable.Range(4, 6)); // 4..9, 4..9;

			var joinedData1 = data1.FullOuterJoin(data2, left => left, right => right, (left, right) => new { Left = left, Right = right }).ToList();
			var joinedData2 = data1.FullOuterJoin(data2, left => left, right => right, (left, right) => 0).ToList();
			Assert.AreEqual(22, joinedData1.Count); // 5ka je v obou kolekcích
			Assert.AreEqual(22, joinedData2.Count); // 5ka je v obou kolekcích
		}

		[TestMethod]
		public void EnumerableExt_SkipLast()
		{
			int count1 = Enumerable.Range(1, 5).SkipLast(3).Count(); //1..5
			int count2 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)).SkipLast(3).Count(); //1..5,1..5
			int count3 = Enumerable.Range(1, 5).SkipLast(1000).Count(); //1..5
			int count4 = Enumerable.Range(1, 5).SkipLast(0).Count(); //1..5
			int count5 = Enumerable.Range(1, 5).SkipLast(-5).Count(); //1..5
			int first = Enumerable.Range(1, 5).SkipLast(3).First();
			int last = Enumerable.Range(1, 5).SkipLast(3).Last();

			Assert.AreEqual(2, count1);
			Assert.AreEqual(7, count2);
			Assert.AreEqual(0, count3);
			Assert.AreEqual(5, count4);
			Assert.AreEqual(5, count5);
			Assert.AreEqual(1, first);
			Assert.AreEqual(2, last);

			count1 = Enumerable.Range(1, 5).ToList().SkipLast(3).Count(); //1..5
			count2 = Enumerable.Range(1, 5).ToList().Concat(Enumerable.Range(1, 5)).SkipLast(3).Count(); //1..5,1..5
			count3 = Enumerable.Range(1, 5).ToList().SkipLast(1000).Count(); //1..5
			count4 = Enumerable.Range(1, 5).ToList().SkipLast(0).Count(); //1..5
			count5 = Enumerable.Range(1, 5).ToList().SkipLast(-5).Count(); //1..5
			first = Enumerable.Range(1, 5).ToList().SkipLast(3).First();
			last = Enumerable.Range(1, 5).ToList().SkipLast(3).Last();

			Assert.AreEqual(2, count1);
			Assert.AreEqual(7, count2);
			Assert.AreEqual(0, count3);
			Assert.AreEqual(5, count4);
			Assert.AreEqual(5, count5);
			Assert.AreEqual(1, first);
			Assert.AreEqual(2, last);
		}

		[TestMethod]
		public void EnumerableExt_SkipLastWhile()
		{
			int count1 = Enumerable.Range(1, 5).SkipLastWhile(item => item == 5).Count(); //1..5
			int count2 = Enumerable.Range(1, 5).SkipLastWhile(item => item >= 3).Count(); //1..5
			int count3 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)).SkipLastWhile(item => item == 5).Count(); //1..5,1..5
			int count4 = Enumerable.Range(1, 5).Concat(Enumerable.Range(1, 5)).SkipLastWhile(item => item >= 3).Count(); //1..5,1..5

			int first = Enumerable.Range(1, 5).SkipLastWhile(item => item >= 3).First();
			int last = Enumerable.Range(1, 5).SkipLastWhile(item => item >= 3).Last();

			Assert.AreEqual(4, count1);
			Assert.AreEqual(2, count2);
			Assert.AreEqual(9, count3);
			Assert.AreEqual(7, count4);
			Assert.AreEqual(1, first);
			Assert.AreEqual(2, last);
		}

        [TestMethod]
	    public void Enumerable_ContainsAll_ReturnsTrueWhenSourceContainsAllLookupItems()
	    {
	        // Arrange
	        int[] source = { 1, 2 };
	        int[] lookupItems = { 1 };
	        
            // Act
	        bool result = source.ContainsAll(lookupItems);
	        
            // Assert
            Assert.IsTrue(result);
	    }

	    [TestMethod]
	    public void Enumerable_ContainsAll_ReturnsTrueWhenSourceDoesNotContainAllLookupItems()
	    {
	        // Arrange
	        int[] source = { 1 };
	        int[] lookupItems = { 1, 2 };

	        // Act
	        bool result = source.ContainsAll(lookupItems);

	        // Assert
	        Assert.IsFalse(result);
	    }

	    [TestMethod]
	    public void Enumerable_ContainsAll_ReturnsTrueWhenLookupItemsIsEmpty()
	    {
	        // Arrange
	        int[] source = { 1 };
	        int[] lookupItems = { };

	        // Act
	        bool result = source.ContainsAll(lookupItems);

	        // Assert
	        Assert.IsTrue(result);
	    }

    }
}
