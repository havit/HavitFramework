using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Havit.Model.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal
{
	[TestClass]
	public class PropertyLoadSequenceResolverIncludingDeletedFilteringSubstitutionTests
	{
		[TestMethod]
		public void PropertyLoadSequenceResolverIncludingDeletedFilteringSubstitutionTest_GetPropertiesToLoad_ReturnsPropertyPathsWithCollection()
		{
			PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution resolver = new PropertyLoadSequenceResolverIncludingDeletedFilteringCollectionsSubstitution();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Master item) => item.Children);

			Assert.AreEqual(1, propertiesToLoad.Length);

			Assert.AreEqual(typeof(Master), propertiesToLoad[0].SourceType);
			Assert.AreEqual(nameof(Master.ChildrenIncludingDeleted), propertiesToLoad[0].PropertyName);
			Assert.AreEqual(nameof(Master.Children), propertiesToLoad[0].OriginalPropertyName);
			Assert.AreEqual(typeof(List<Child>), propertiesToLoad[0].TargetType);
			Assert.AreEqual(typeof(FilteringCollection<Child>), propertiesToLoad[0].OriginalTargetType);
			Assert.AreEqual(typeof(Child), propertiesToLoad[0].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[0].IsCollection);
		}
	}
}
