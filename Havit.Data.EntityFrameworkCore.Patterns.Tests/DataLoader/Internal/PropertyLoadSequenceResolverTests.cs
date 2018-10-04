using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal
{
	[TestClass]
	public class PropertyLoadSequenceResolverTests
	{
		[TestMethod]
		public void PropertyLoadSequenceResolver_GetPropertiesToLoad_ReturnsSimplePropertyPaths()
		{
			PropertyLoadSequenceResolver resolver = new PropertyLoadSequenceResolver();
			PropertyToLoad[] propertiesToLoad = resolver.GetPropertiesToLoad((Child child) => child.Parent.Children);

			Assert.AreEqual(2, propertiesToLoad.Length);

			Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
			Assert.IsFalse(propertiesToLoad[0].IsCollection);

			Assert.AreEqual(typeof(Master), propertiesToLoad[1].SourceType);
			Assert.AreEqual(typeof(ICollection<Child>), propertiesToLoad[1].TargetType);
			Assert.AreEqual(typeof(Child), propertiesToLoad[1].CollectionItemType);
			Assert.AreEqual(nameof(Master.Children), propertiesToLoad[1].PropertyName);
			Assert.IsTrue(propertiesToLoad[1].IsCollection);

		}
	}
}
