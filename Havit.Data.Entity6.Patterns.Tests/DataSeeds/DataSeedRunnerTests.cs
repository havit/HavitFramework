using Havit.Data.Entity.Patterns.DataSeeds;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Helpers;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class DataSeedRunnerTests
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
		public void DataSeedRunner_SeedData_SupportsPairingByNullableProperties()
		{
			// Arrange
			DbDataSeedPersister dataSeedPersister = new DbDataSeedPersister(new TestDbContext());

			Mock<IServiceFactory<IDataSeedPersister>> dataSeedPersisterFactoryMock = new Mock<IServiceFactory<IDataSeedPersister>>(MockBehavior.Strict);
			dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersister);
			dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

			DataSeedRunner dataSeedRunner = new DataSeedRunner(new IDataSeed[] { new ItemWithNullablePropertySeed() }, new AlwaysRunDecision(), dataSeedPersisterFactoryMock.Object);

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
}
