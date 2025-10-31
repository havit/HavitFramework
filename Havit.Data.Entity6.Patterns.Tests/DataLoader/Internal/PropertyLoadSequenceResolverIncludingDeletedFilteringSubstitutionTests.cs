using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class PropertyLoadSequenceResolverIncludingDeletedFilteringSubstitutionTests
{
	[TestMethod]
	public void PropertyLoadSequenceResolverIncludingDeletedFilteringSubstitutionTest_GetPropertiesToLoad_ReturnsPropertyPathsWithCollection()
	{
		PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution resolver = new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution();
		PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Master item) => item.Children.Select(child => child.Parent.Children));

		Assert.HasCount(3, propertiesToLoad);

		Assert.AreEqual(nameof(Master.ChildrenIncludingDeleted), propertiesToLoad[0].PropertyName);
		Assert.AreEqual(typeof(Master), propertiesToLoad[0].SourceType);
		Assert.AreEqual(typeof(List<Child>), propertiesToLoad[0].TargetType);
		Assert.AreEqual(typeof(Child), propertiesToLoad[0].CollectionItemType);
		Assert.IsTrue(propertiesToLoad[0].IsCollection);

		Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[1].PropertyName);
		Assert.AreEqual(typeof(Child), propertiesToLoad[1].SourceType);
		Assert.AreEqual(typeof(Master), propertiesToLoad[1].TargetType);
		Assert.IsFalse(propertiesToLoad[1].IsCollection);
		Assert.IsNull(propertiesToLoad[1].CollectionItemType);

		Assert.AreEqual(nameof(Master.ChildrenIncludingDeleted), propertiesToLoad[2].PropertyName);
		Assert.AreEqual(typeof(Master), propertiesToLoad[2].SourceType);
		Assert.AreEqual(typeof(List<Child>), propertiesToLoad[2].TargetType);
		Assert.AreEqual(typeof(Child), propertiesToLoad[2].CollectionItemType);
		Assert.IsTrue(propertiesToLoad[2].IsCollection);

	}
}
