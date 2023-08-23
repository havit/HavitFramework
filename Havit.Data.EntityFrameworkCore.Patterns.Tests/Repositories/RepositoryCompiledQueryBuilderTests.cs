using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Repositories;
[TestClass]
public class RepositoryCompiledQueryBuilderTests
{
	[TestMethod]
	public void RepositoryCompiledQueryBuilder_Constructor_InitializesFields()
	{
		// Arrange
		// NOOP

		// Act
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();

		// Assert
		Assert.IsNotNull(compiledQueryBuilder.WhereMethod);
		Assert.IsNotNull(compiledQueryBuilder.FirstOrDefaultMethod);
		Assert.IsNotNull(compiledQueryBuilder.TagWithMethod);
		Assert.IsNotNull(compiledQueryBuilder.DbContextSetMethod);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetObjectCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetObjectAsyncCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectAsyncCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetObjectsCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectsCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetObjectsAsyncCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();

		// Act
		var result = compiledQueryBuilder.CreateGetObjectsAsyncCompiledQuery<Person>(typeof(IRepository<Person>), GetEntityKeyAccessorMock());

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetAllCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();
		var softDeleteManagerMock = new Mock<ISoftDeleteManager>(MockBehavior.Strict);

		// Act
		var result = compiledQueryBuilder.CreateGetAllCompiledQuery<Person>(typeof(IRepository<Person>), softDeleteManagerMock.Object);

		// Assert (smoke test)
		Assert.IsNotNull(result);
	}

	[TestMethod]
	public void RepositoryCompiledQueryBuilder_CreateGetAllAsyncCompiledQuery()
	{
		// Arrange
		RepositoryCompiledQueryBuilder compiledQueryBuilder = new RepositoryCompiledQueryBuilder();
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