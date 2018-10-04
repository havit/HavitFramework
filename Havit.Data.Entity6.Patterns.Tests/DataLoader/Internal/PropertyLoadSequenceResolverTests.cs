using Havit.Data.Entity.Patterns.DataLoaders.Internal;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Internal
{
	[TestClass]
	public class PropertyLoadSequenceResolverTests
	{
		[TestMethod]
		public void PropertyLoadSequenceResolver_GetPropertiesToLoad_ReturnsSimplePropertyPaths()
		{
			PropertyLoadSequenceResolver resolver = new PropertyLoadSequenceResolver();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Child child) => child.Parent);

			Assert.AreEqual(1, propertiesToLoad.Length);

			Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
			Assert.IsFalse(propertiesToLoad[0].IsCollection);
		}

		[TestMethod]
		public void PropertyLoadSequenceResolver_GetPropertiesToLoad_ReturnsPropertyPathsWithCollection()
		{
			PropertyLoadSequenceResolver resolver = new PropertyLoadSequenceResolver();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Child child) => child.Parent.Children.Select(item => item.Parent));

			Assert.AreEqual(3, propertiesToLoad.Length);

			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
			Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
			Assert.IsFalse(propertiesToLoad[0].IsCollection);
			Assert.IsNull(propertiesToLoad[0].CollectionItemType);

			Assert.AreEqual(nameof(Master.Children), propertiesToLoad[1].PropertyName);
			Assert.AreEqual(typeof(Master), propertiesToLoad[1].SourceType);
			Assert.AreEqual(typeof(ICollection<Child>), propertiesToLoad[1].TargetType);
			Assert.AreEqual(typeof(Child), propertiesToLoad[1].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[1].IsCollection);

			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[2].PropertyName);
			Assert.AreEqual(typeof(Child), propertiesToLoad[2].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[2].TargetType);
			Assert.IsFalse(propertiesToLoad[2].IsCollection);
			Assert.IsNull(propertiesToLoad[2].CollectionItemType);
		}

		[TestMethod]
		public void PropertyLoadSequenceResolver_GetPropertiesToLoad_ReturnsPropertyPathsForHierarchy()
		{
			PropertyLoadSequenceResolver resolver = new PropertyLoadSequenceResolver();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((HiearchyItem item) => item.Children.Select(item2 => item2.Children.Select(item3 => item3.Children)));

			Assert.AreEqual(3, propertiesToLoad.Length);

			Assert.AreEqual(nameof(HiearchyItem.Children), propertiesToLoad[0].PropertyName);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(ICollection<HiearchyItem>), propertiesToLoad[0].TargetType);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[0].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[0].IsCollection);

			Assert.AreEqual(nameof(HiearchyItem.Children), propertiesToLoad[1].PropertyName);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[1].SourceType);
			Assert.AreEqual(typeof(ICollection<HiearchyItem>), propertiesToLoad[1].TargetType);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[1].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[1].IsCollection);

			Assert.AreEqual(nameof(HiearchyItem.Children), propertiesToLoad[2].PropertyName);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[2].SourceType);
			Assert.AreEqual(typeof(ICollection<HiearchyItem>), propertiesToLoad[2].TargetType);
			Assert.AreEqual(typeof(HiearchyItem), propertiesToLoad[2].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[2].IsCollection);
		}

	}
}
