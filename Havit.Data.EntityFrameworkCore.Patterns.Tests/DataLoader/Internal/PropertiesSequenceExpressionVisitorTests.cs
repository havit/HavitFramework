using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class PropertiesSequenceExpressionVisitorTests
{
	private class TestEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public byte[] Data { get; set; }
		public List<TestEntity> Children { get; set; }
	}

	[TestMethod]
	public void PropertiesSequenceExpressionVisitor_GetPropertiesToLoad_StringIsNotClassifiedAsCollection()
	{
		// Act
		PropertyToLoad[] propertiesToLoad = new PropertiesSequenceExpressionVisitor().GetPropertiesToLoad((TestEntity entity) => entity.Name);

		// Assert
		Assert.HasCount(1, propertiesToLoad);
		Assert.AreEqual(nameof(TestEntity.Name), propertiesToLoad[0].PropertyName);
		Assert.IsFalse(propertiesToLoad[0].IsCollection, "string (IEnumerable<char>) nesmí být klasifikován jako kolekce.");
		Assert.IsNull(propertiesToLoad[0].CollectionItemType);
	}

	[TestMethod]
	public void PropertiesSequenceExpressionVisitor_GetPropertiesToLoad_ByteArrayIsNotClassifiedAsCollection()
	{
		// Act
		PropertyToLoad[] propertiesToLoad = new PropertiesSequenceExpressionVisitor().GetPropertiesToLoad((TestEntity entity) => entity.Data);

		// Assert
		Assert.HasCount(1, propertiesToLoad);
		Assert.AreEqual(nameof(TestEntity.Data), propertiesToLoad[0].PropertyName);
		Assert.IsFalse(propertiesToLoad[0].IsCollection, "byte[] (IEnumerable<byte>) nesmí být klasifikován jako kolekce.");
		Assert.IsNull(propertiesToLoad[0].CollectionItemType);
	}

	[TestMethod]
	public void PropertiesSequenceExpressionVisitor_GetPropertiesToLoad_GenericCollectionIsClassifiedAsCollection()
	{
		// Act
		PropertyToLoad[] propertiesToLoad = new PropertiesSequenceExpressionVisitor().GetPropertiesToLoad((TestEntity entity) => entity.Children);

		// Assert
		Assert.HasCount(1, propertiesToLoad);
		Assert.AreEqual(nameof(TestEntity.Children), propertiesToLoad[0].PropertyName);
		Assert.IsTrue(propertiesToLoad[0].IsCollection);
		Assert.AreEqual(typeof(TestEntity), propertiesToLoad[0].CollectionItemType);
	}
}
