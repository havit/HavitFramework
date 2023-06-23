using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Repositories;
[TestClass]
public class DbRepositoryCompiledQueryBuilderTests
{
	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_Constructor_InitializesFields()
	{
		// Arrange
		// NOOP

		// Act
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

		// Assert
		Assert.IsNotNull(compiledQueryBuilder.whereMethod);
		Assert.IsNotNull(compiledQueryBuilder.firstOrDefaultMethod);
		Assert.IsNotNull(compiledQueryBuilder.tagWithMethod);
		Assert.IsNotNull(compiledQueryBuilder.dbContextSetMethod);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetObjectCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetObjectAsyncCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectAsyncCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetObjectsCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectsCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetObjectsAsyncCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectsAsyncCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetAllCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();
		var softDeleteManagerMock = new Mock<ISoftDeleteManager>(MockBehavior.Strict);

		// Act
		var result = compiledQueryBuilder.CreateGetAllCompiledQuery<Person>(typeof(IRepository<Person>), softDeleteManagerMock.Object);

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_CreateGetAllAsyncCompiledQuery()
	{
		// Arrange
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();
		var softDeleteManagerMock = new Mock<ISoftDeleteManager>(MockBehavior.Strict);

		// Act
		var result = compiledQueryBuilder.CreateGetAllAsyncCompiledQuery<Person>(typeof(IRepository<Person>), softDeleteManagerMock.Object);

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	private IEntityKeyAccessor<Person, int> GetEntityKeyAccessorMock()
	{
		var entityKeyAccessorMock = new Mock<IEntityKeyAccessor<Person, int>>(MockBehavior.Strict);
		entityKeyAccessorMock.Setup(mock => mock.GetEntityKeyPropertyName()).Returns(nameof(Person.Id));
		return entityKeyAccessorMock.Object;
	}

	public class Person
	{
		public int Id { get; set; }
	}
}