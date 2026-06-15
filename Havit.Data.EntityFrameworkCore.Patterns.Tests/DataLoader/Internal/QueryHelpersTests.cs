using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class QueryHelpersTests
{
	[TestMethod]
	public void QueryHelpers_ContainsEffectiveInteger_Zero()
	{
		// Act
		var result = GetData().Where(new List<int> { }.ContainsEffectiveInteger<TestClass, int>(p => p.Id)).ToList();

		// Assert
		Assert.IsFalse(result.Any());
	}

	[TestMethod]
	public void QueryHelpers_ContainsEffectiveInteger_One()
	{
		// Act
		var result = GetData().Where(new List<int> { 2 }.ContainsEffectiveInteger<TestClass, int>(p => p.Id)).ToList();

		// Assert
		Assert.HasCount(1, result, "Count");
		Assert.IsTrue(result.Any(item => item.Id == 2), "2");
	}

	[TestMethod]
	public void QueryHelpers_ContainsEffectiveInteger_Sorted()
	{
		// Act
		var result = GetData().Where(new List<int> { 2, 3, 4 }.ContainsEffectiveInteger<TestClass, int>(p => p.Id)).ToList();

		// Assert
		Assert.HasCount(3, result, "Count");
		Assert.IsTrue(result.Any(item => item.Id == 2), "2");
		Assert.IsTrue(result.Any(item => item.Id == 3), "3");
		Assert.IsTrue(result.Any(item => item.Id == 4), "4");
	}

	[TestMethod]
	public void QueryHelpers_ContainsEffectiveInteger_NotSorted()
	{
		// Act
		var result = GetData().Where(new List<int> { 1, 2, 3, 5 }.ContainsEffectiveInteger<TestClass, int>(p => p.Id)).ToList();

		// Assert
		Assert.HasCount(4, result, "Count");
		Assert.IsTrue(result.Any(item => item.Id == 1), "1");
		Assert.IsTrue(result.Any(item => item.Id == 2), "2");
		Assert.IsTrue(result.Any(item => item.Id == 3), "3");
		Assert.IsTrue(result.Any(item => item.Id == 5), "5");
	}

	[TestMethod]
	public void QueryHelpers_TryGetConsecutiveRange_SortedSequence()
	{
		// Act
		var result = QueryHelpers.TryGetConsecutiveRange(new List<int> { 1, 2, 3, 4, 4, 5 }, out int minValue, out int maxValue);

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(1, minValue);
		Assert.AreEqual(5, maxValue);
	}

	[TestMethod]
	public void QueryHelpers_TryGetConsecutiveRange_UnsortedSequence()
	{
		// Act
		var result = QueryHelpers.TryGetConsecutiveRange(new List<int> { 4, 5, 4, 2, 3, 1 }, out int minValue, out int maxValue);

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(1, minValue);
		Assert.AreEqual(5, maxValue);
	}

	[TestMethod]
	public void QueryHelpers_TryGetConsecutiveRange_NotSequence()
	{
		// Act
		var result = QueryHelpers.TryGetConsecutiveRange(new List<int> { 4, 5, 4, 3, 1 }, out int _, out int _);

		// Assert
		Assert.IsFalse(result);
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
