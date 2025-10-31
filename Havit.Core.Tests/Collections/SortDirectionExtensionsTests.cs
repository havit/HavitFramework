using Havit.Collections;

namespace Havit.Tests.Collections;

[TestClass]
public class SortDirectionExtensionsTests
{
	[TestMethod]
	public void SortDirectionExtensions_Reverse()
	{
		// Arrange + Assert
		Assert.AreEqual(SortDirection.Descending, SortDirection.Ascending.Reverse());
		Assert.AreEqual(SortDirection.Ascending, SortDirection.Descending.Reverse());
	}
}
