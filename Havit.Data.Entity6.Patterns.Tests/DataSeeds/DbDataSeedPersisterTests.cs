using Havit.Data.Entity.Patterns.DataSeeds;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.DataSeeds;
using Moq;
using System.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.Tests.DataSeeds;

[TestClass]
public class DbDataSeedPersisterTests
{
	[TestMethod]
	public void DbDataSeedPersister_Save_ThrowsExceptionWhenPairingNotDefined()
	{
		// Arrange
		Mock<IDbContext> dbContextMock = new Mock<IDbContext>();

		DbDataSeedPersister dbDataSeedPersister = new DbDataSeedPersister(dbContextMock.Object);

		DataSeedConfiguration<Language> dataSeedConfiguration = new DataSeedConfiguration<Language>(new Language[] { });

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			dbDataSeedPersister.Save(dataSeedConfiguration);
		});
	}

	[TestMethod]
	public void DbDataSeedPersister_PairWithDbData_ThrowsExceptionWhenPairedToMoreThanOneObject()
	{
		// Arrange
		Mock<IDbContext> dbContextMock = new Mock<IDbContext>();

		DbDataSeedPersister dbDataSeedPersister = new DbDataSeedPersister(dbContextMock.Object);

		DataSeedConfiguration<Language> dataSeedConfiguration = new DataSeedConfiguration<Language>(new Language[] { new Language() { Id = 1 } });
		dataSeedConfiguration.PairByExpressions = new List<Expression<Func<Language, object>>> { l => l.Id };

		Language language1 = new Language() { Id = 1 };
		Language language2 = new Language() { Id = 1 };
		IQueryable<Language> languagesQueryable = new Language[] { language1, language2 }.AsQueryable();

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			dbDataSeedPersister.PairWithDbData(languagesQueryable, dataSeedConfiguration);
		});

	}
}
