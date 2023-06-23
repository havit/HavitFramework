using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Repositories;
[TestClass]
public class DbRepositoryCompiledQueryBuilderTests
{
	[TestMethod]
	public void DbRepositoryCompiledQueryBuilder_EnsureMethodInfos_InitializesFields()
	{
		// Arrange
		// noop

		// Act
		DbRepositoryCompiledQueryBuilder compiledQueryBuilder = new DbRepositoryCompiledQueryBuilder();
		compiledQueryBuilder.EnsureMethodInfos();

		// Assert
		Assert.IsNotNull(compiledQueryBuilder.whereMethod);
		Assert.IsNotNull(compiledQueryBuilder.firstOrDefaultMethod);
		Assert.IsNotNull(compiledQueryBuilder.tagWithMethod);
		Assert.IsNotNull(compiledQueryBuilder.dbContextSetMethod);
	}
}