using System.Linq.Expressions;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Internal;

[TestClass]
public class PropertiesSequenceExpressionVisitorTests
{
	private interface ITestEntity
	{
		TestEntity Parent { get; set; }
	}

	private interface ITestEntityWithExplicitProperty
	{
		TestEntity ParentExplicit { get; }
	}

	private class TestEntity : ITestEntity, ITestEntityWithExplicitProperty
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public byte[] Data { get; set; }
		public List<TestEntity> Children { get; set; }
		public TestEntity Parent { get; set; }

		TestEntity ITestEntityWithExplicitProperty.ParentExplicit => Parent;
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

	[TestMethod]
	public void PropertiesSequenceExpressionVisitor_GetPropertiesToLoad_InterfaceBoundPropertyIsRemappedToImplementingType()
	{
		// Arrange
		// Lambda vznikla v generické metodě s type parametrem omezeným interfacem - vlastnost Parent je proto
		// v expression tree navázána na ITestEntity, přestože parametr lambdy je typu TestEntity.
		Expression<Func<TestEntity, TestEntity>> propertyPath = CreatePropertyPathWithInterfaceBoundProperty<TestEntity>();

		// Act
		PropertyToLoad[] propertiesToLoad = new PropertiesSequenceExpressionVisitor().GetPropertiesToLoad(propertyPath);

		// Assert
		Assert.HasCount(1, propertiesToLoad);
		Assert.AreEqual(typeof(TestEntity), propertiesToLoad[0].SourceType, "SourceType musí být typ entity, nikoliv interface, na který je vlastnost navázána v expression tree.");
		Assert.AreEqual(nameof(TestEntity.Parent), propertiesToLoad[0].PropertyName);
		Assert.AreEqual(typeof(TestEntity), propertiesToLoad[0].TargetType);
	}

	[TestMethod]
	public void PropertiesSequenceExpressionVisitor_GetPropertiesToLoad_ThrowsForExplicitlyImplementedInterfaceBoundProperty()
	{
		// Arrange
		Expression<Func<TestEntity, TestEntity>> propertyPath = CreatePropertyPathWithExplicitlyImplementedInterfaceBoundProperty<TestEntity>();

		// Act + Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			new PropertiesSequenceExpressionVisitor().GetPropertiesToLoad(propertyPath);
		});
	}

	private static Expression<Func<TEntity, TestEntity>> CreatePropertyPathWithInterfaceBoundProperty<TEntity>()
		where TEntity : class, ITestEntity
	{
		return entity => entity.Parent;
	}

	private static Expression<Func<TEntity, TestEntity>> CreatePropertyPathWithExplicitlyImplementedInterfaceBoundProperty<TEntity>()
		where TEntity : class, ITestEntityWithExplicitProperty
	{
		return entity => entity.ParentExplicit;
	}
}
