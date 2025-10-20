using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Model.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class PropertyLoadSequenceResolverTests
{
	[TestMethod]
	public void PropertyLoadSequenceResolver_GetPropertiesToLoad_ReturnsSimplePropertyPaths()
	{
		PropertyLoadSequenceResolver resolver = new PropertyLoadSequenceResolver();
		PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Child child) => child.Parent.Children);

		Assert.HasCount(2, propertiesToLoad);

		Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
		Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
		Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].OriginalPropertyName);
		Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
		Assert.AreEqual(typeof(Master), propertiesToLoad[0].OriginalTargetType);
		Assert.IsFalse(propertiesToLoad[0].IsCollection);

		Assert.AreEqual(typeof(Master), propertiesToLoad[1].SourceType);
		Assert.AreEqual(nameof(Master.Children), propertiesToLoad[1].PropertyName);
		Assert.AreEqual(nameof(Master.Children), propertiesToLoad[1].OriginalPropertyName);
		Assert.AreEqual(typeof(FilteringCollection<Child>), propertiesToLoad[1].TargetType);
		Assert.AreEqual(typeof(FilteringCollection<Child>), propertiesToLoad[1].OriginalTargetType);
		Assert.AreEqual(typeof(Child), propertiesToLoad[1].CollectionItemType);
		Assert.IsTrue(propertiesToLoad[1].IsCollection);

	}
}
