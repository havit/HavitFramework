using Havit.Data.Entity.Patterns.DataSeeds;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Patterns.Tests.Helpers;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Moq;
using Havit.Data.Entity.Patterns.Transactions.Internal;

namespace Havit.Data.Entity.Patterns.Tests.DataSeeds;

[TestClass]
public class DbDataSeedRunnerTests
{
	[ClassCleanup]
	public static void CleanUp()
	{
		DeleteDatabaseHelper.DeleteDatabase<TestDbContext>();
	}

	/// <summary>
	/// Reprodukce bugu #35740
	/// </summary>
	[TestMethod]
	public void DbDataSeedRunner_SeedData_SupportsPairingByNullableProperties()
	{
		// Arrange
		new TestDbContext().Database.Initialize(false); // DROP&CREATE nelze udělat uvnitř transaction scope - viz použití TransactionScopeTransactionWrapper

		DbDataSeedPersister dataSeedPersister = new DbDataSeedPersister(new TestDbContext());

		Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
		dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersister);
		dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

		DbDataSeedRunner dataSeedRunner = new DbDataSeedRunner(new IDataSeed[] { new ItemWithNullablePropertySeed() }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object, new TransactionScopeTransactionWrapper());

		// Act
		dataSeedRunner.SeedData<DefaultProfile>();

		// Assert
		// no exception thrown
	}

	// nested class
	public class ItemWithNullablePropertySeed : DataSeed<DefaultProfile>
	{
		public override void SeedData()
		{
			//ItemWithNullableProperty
			Seed(For(new ItemWithNullableProperty { NullableValue = 1 }).PairBy(item => item.NullableValue));
		}
	}

}
