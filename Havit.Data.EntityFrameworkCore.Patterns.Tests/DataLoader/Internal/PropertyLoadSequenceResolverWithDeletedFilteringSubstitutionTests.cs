using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal
{
	[TestClass]
	public class PropertyLoadSequenceResolverWithDeletedFilteringSubstitutionTests
	{
		[TestMethod]
		public void PropertyLoadSequenceResolverWithDeletedFilteringSubstitutionTest_GetPropertiesToLoad_ReturnsPropertyPathsWithCollection()
		{
			PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution resolver = new PropertyLoadSequenceResolverWithDeletedFilteringCollectionsSubstitution();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Master item) => item.Children);

			Assert.AreEqual(1, propertiesToLoad.Length);

			Assert.AreEqual(nameof(Master.ChildrenWithDeleted), propertiesToLoad[0].PropertyName);
			Assert.AreEqual(nameof(Master.Children), propertiesToLoad[0].OriginalPropertyName);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(List<Child>), propertiesToLoad[0].TargetType);
			Assert.AreEqual(typeof(Child), propertiesToLoad[0].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[0].IsCollection);
		}
	}
}
