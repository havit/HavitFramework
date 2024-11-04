using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class DbLoadedPropertyReaderWithMemoryTests
{
	[TestMethod]
	public void DbLoadedPropertyReaderWithMemory_IsEntityPropertyLoaded()
	{
		// Arrange
		DbLoadedPropertyReaderWithMemory dbLoadedPropertyReaderWithMemory = new DbLoadedPropertyReaderWithMemory(new DataLoaderTestDbContext());
		var hiararchyItem1 = new HiearchyItem();
		var hiararchyItem2 = new HiearchyItem();

		// Act + Assert
		Assert.IsFalse(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem1, nameof(HiearchyItem.Parent)));
		Assert.IsTrue(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem1, nameof(HiearchyItem.Parent)));
		Assert.IsTrue(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem1, nameof(HiearchyItem.Parent)));

		// another property
		Assert.IsFalse(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem1, nameof(HiearchyItem.Children)));
		Assert.IsTrue(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem1, nameof(HiearchyItem.Children)));

		// another entity
		Assert.IsFalse(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem2, nameof(HiearchyItem.Children)));
		Assert.IsTrue(dbLoadedPropertyReaderWithMemory.IsEntityPropertyLoaded(hiararchyItem2, nameof(HiearchyItem.Children)));
	}
}
