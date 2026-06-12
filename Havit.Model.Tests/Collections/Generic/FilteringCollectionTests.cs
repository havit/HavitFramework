using Havit.Model.Collections.Generic;
using System.Collections.ObjectModel;

namespace Havit.Model.Tests.Collections.Generic;

[TestClass]
public class FilteringCollectionTests
{
	[TestMethod]
	public void FilteringCollection_List_Add()
	{
		// Arrange
		var list = new List<int>();
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Add(1);
		filteringCollection.Add(2);

		// Assert
		Assert.Contains(1, list);
		Assert.Contains(2, list);
	}

	[TestMethod]
	public void FilteringCollection_List_AddRange()
	{
		// Arrange
		var list = new List<int>();
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.AddRange(new List<int> { 1, 2 });

		// Assert
		Assert.Contains(1, list);
		Assert.Contains(2, list);
	}

	[TestMethod]
	public void FilteringCollection_List_Clear()
	{
		// Arrange
		var list = new List<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Clear();

		// Assert
		Assert.IsEmpty(list);
	}
	[TestMethod]
	public void FilteringCollection_List_Contains()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act + Assert
		Assert.IsFalse(filteringCollection.Contains(1));
		Assert.IsTrue(filteringCollection.Contains(2));
	}

	[TestMethod]
	public void FilteringCollection_List_Count()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act + Assert
		Assert.AreEqual(2, filteringCollection.Count);
	}


	[TestMethod]
	public void FilteringCollection_List_ForEach()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act
		int sum = 0;
		filteringCollection.ForEach(i => sum += i);

		// Assert
		Assert.AreEqual(6, sum);
	}

	[TestMethod]
	public void FilteringCollection_List_Remove()
	{
		// Arrange
		var list = new List<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Remove(1);
		filteringCollection.Remove(2);

		// Assert
		Assert.DoesNotContain(1, list);
		Assert.DoesNotContain(2, list);
		Assert.Contains(3, list);
		Assert.Contains(4, list);
	}

	[TestMethod]
	public void FilteringCollection_List_RemoveAll()
	{
		// Arrange
		var list = new List<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		var result = filteringCollection.RemoveAll(i => i < 3);

		// Assert
		Assert.AreEqual(2, result);
		Assert.DoesNotContain(1, list);
		Assert.DoesNotContain(2, list);
		Assert.Contains(3, list);
		Assert.Contains(4, list);
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_Add()
	{
		// Arrange
		var list = new ObservableCollection<int>();
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Add(1);
		filteringCollection.Add(2);

		// Assert
		Assert.Contains(1, list);
		Assert.Contains(2, list);
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_AddRange()
	{
		// Arrange
		var list = new ObservableCollection<int>();
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.AddRange(new ObservableCollection<int> { 1, 2 });

		// Assert
		Assert.Contains(1, list);
		Assert.Contains(2, list);
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_Clear()
	{
		// Arrange
		var list = new ObservableCollection<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Clear();

		// Assert
		Assert.IsEmpty(list);
	}
	[TestMethod]
	public void FilteringCollection_ObservableCollection_Contains()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new ObservableCollection<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act + Assert
		Assert.IsFalse(filteringCollection.Contains(1));
		Assert.IsTrue(filteringCollection.Contains(2));
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_Count()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new ObservableCollection<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act + Assert
		Assert.AreEqual(2, filteringCollection.Count);
	}


	[TestMethod]
	public void FilteringCollection_ObservableCollection_ForEach()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new ObservableCollection<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act
		int sum = 0;
		filteringCollection.ForEach(i => sum += i);

		// Assert
		Assert.AreEqual(6, sum);
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_Remove()
	{
		// Arrange
		var list = new ObservableCollection<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Remove(1);
		filteringCollection.Remove(2);

		// Assert
		Assert.DoesNotContain(1, list);
		Assert.DoesNotContain(2, list);
		Assert.Contains(3, list);
		Assert.Contains(4, list);
	}

	[TestMethod]
	public void FilteringCollection_ObservableCollection_RemoveAll()
	{
		// Arrange
		var list = new ObservableCollection<int>() { 1, 2, 3, 4 };
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		var result = filteringCollection.RemoveAll(i => i < 3);

		// Assert
		Assert.AreEqual(2, result);
		Assert.DoesNotContain(1, list);
		Assert.DoesNotContain(2, list);
		Assert.Contains(3, list);
		Assert.Contains(4, list);
	}

	[TestMethod]
	public void FilteringCollection_CopyTo_CopiesOnlyFilteredItems()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);
		var target = new int[2];

		// Act
		filteringCollection.CopyTo(target, 0);

		// Assert
		CollectionAssert.AreEqual(new[] { 2, 4 }, target);
	}

	[TestMethod]
	public void FilteringCollection_CopyTo_RespectsArrayIndex()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);
		var target = new int[3];

		// Act
		filteringCollection.CopyTo(target, 1);

		// Assert
		CollectionAssert.AreEqual(new[] { 0, 2, 4 }, target);
	}

	[TestMethod]
	public void FilteringCollection_IsReadOnly_ReturnsFalse()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int>(), i => i % 2 == 0);

		// Act + Assert
		Assert.IsFalse(filteringCollection.IsReadOnly);
	}

	[TestMethod]
	public void FilteringCollection_Add_ItemNotMatchingFilter_StaysInSourceButIsHidden()
	{
		// Arrange
		var list = new List<int>();
		var filteringCollection = new FilteringCollection<int>(list, i => i % 2 == 0);

		// Act
		filteringCollection.Add(1); // liché číslo - filtrem neprojde

		// Assert
		Assert.Contains(1, list); // do podkladové kolekce se přidá bez ohledu na filtr
		Assert.IsFalse(filteringCollection.Contains(1)); // ...ale přes filtr není vidět
		Assert.AreEqual(0, filteringCollection.Count);
		Assert.IsEmpty(filteringCollection.ToList()); // enumerace filtr aplikuje
	}

	[TestMethod]
	public void FilteringCollection_GetEnumerator_NonGeneric_ReturnsOnlyFilteredItems()
	{
		// Arrange
		var filteringCollection = new FilteringCollection<int>(new List<int> { 1, 2, 3, 4 }, i => i % 2 == 0);

		// Act
		var result = new List<int>();
		foreach (int item in (System.Collections.IEnumerable)filteringCollection)
		{
			result.Add(item);
		}

		// Assert
		CollectionAssert.AreEqual(new[] { 2, 4 }, result);
	}

	[TestMethod]
	public void FilteringCollection_Constructor_NullSource_Throws()
	{
		// Act + Assert
		Assert.ThrowsExactly<ArgumentNullException>(() => new FilteringCollection<int>(null, i => i % 2 == 0));
	}

	[TestMethod]
	public void FilteringCollection_Constructor_NullFilter_Throws()
	{
		// Act + Assert
		Assert.ThrowsExactly<ArgumentNullException>(() => new FilteringCollection<int>(new List<int>(), null));
	}

}
