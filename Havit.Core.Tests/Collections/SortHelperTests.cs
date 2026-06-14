using System.Collections;
using Havit.Collections;

namespace Havit.Tests.Collections;

[TestClass]
public class SortHelperTests
{
	[TestMethod]
	public void SortHelper_PropertySort_SortsByProperty()
	{
		// Arrange
		List<Item> data = new List<Item>
		{
			new Item { Value = 3 },
			new Item { Value = 1 },
			new Item { Value = 2 }
		};

		// Act
		List<object> result = SortHelper.PropertySort(data, "Value").Cast<object>().ToList();

		// Assert
		CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.Cast<Item>().Select(i => i.Value).ToArray());
	}

	private class Item
	{
		public int Value { get; set; }
	}
}
