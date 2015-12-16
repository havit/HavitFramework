using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataLoaders;
using Havit.Data.Entity.Patterns.Tests.DataLoader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader
{
	[TestClass]
	public class PropertiesSequenceExpressionVisitorTest
	{
		[TestMethod]
		public void PropertiesSequenceExpressionVisitor_GetLoadPlanItems_ReturnsSimplePropertyPaths()
		{
			PropertiesSequenceExpressionVisitor visitor = new PropertiesSequenceExpressionVisitor();
			PropertyToLoad[] propertiesToLoad = visitor.GetPropertiesToLoad((Child child) => child.Parent);

			Assert.AreEqual(1, propertiesToLoad.Length);

			Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
			Assert.IsFalse(propertiesToLoad[0].IsCollection);
		}

		[TestMethod]
		public void PropertiesSequenceExpressionVisitor_GetLoadPlanItems_ReturnsPropertyPathsWithCollection()
		{
			PropertiesSequenceExpressionVisitor visitor = new PropertiesSequenceExpressionVisitor();
			PropertyToLoad[] propertiesToLoad = visitor.GetPropertiesToLoad((Child child) => child.Parent.Children.Unwrap().Parent);

			Assert.AreEqual(3, propertiesToLoad.Length);

			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[0].PropertyName);
			Assert.AreEqual(typeof(Child), propertiesToLoad[0].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[0].TargetType);
			Assert.IsFalse(propertiesToLoad[0].IsCollection);
			Assert.IsNull(propertiesToLoad[0].CollectionItemType);
			Assert.IsFalse(propertiesToLoad[0].CollectionUnwrapped);

			Assert.AreEqual(nameof(Master.Children), propertiesToLoad[1].PropertyName);
			Assert.AreEqual(typeof(Master), propertiesToLoad[1].SourceType);
			Assert.AreEqual(typeof(ICollection<Child>), propertiesToLoad[1].TargetType);
			Assert.AreEqual(typeof(Child), propertiesToLoad[1].CollectionItemType);
			Assert.IsTrue(propertiesToLoad[1].IsCollection);
			Assert.IsTrue(propertiesToLoad[1].CollectionUnwrapped);

			Assert.AreEqual(nameof(Child.Parent), propertiesToLoad[2].PropertyName);
			Assert.AreEqual(typeof(Child), propertiesToLoad[2].SourceType);
			Assert.AreEqual(typeof(Master), propertiesToLoad[2].TargetType);
			Assert.IsFalse(propertiesToLoad[2].IsCollection);
			Assert.IsNull(propertiesToLoad[2].CollectionItemType);
			Assert.IsFalse(propertiesToLoad[2].CollectionUnwrapped);
		}
	}
}
