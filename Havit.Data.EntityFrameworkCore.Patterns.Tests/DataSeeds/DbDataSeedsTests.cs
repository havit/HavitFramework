using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds
{
    public class DbDataSeedsTests
    {
        private static DataSeedRunner GetDefaultDataSeedRunner(IDbContextTransient dbContext, params IDataSeed[] dataSeedsParams)
        {
            IEnumerable<IDataSeed> dataSeeds = new List<IDataSeed>(dataSeedsParams);
            IDataSeedRunDecision dataSeedRunDecision = new AlwaysRunDecision();
            IDataSeedPersister dataSeedPersister = new DbDataSeedPersister(dbContext);

			Mock<IDataSeedPersisterFactory> dataSeedPersisterFactoryMock = new Mock<IDataSeedPersisterFactory>(MockBehavior.Strict);
			dataSeedPersisterFactoryMock.Setup(m => m.CreateService()).Returns(dataSeedPersister);
			dataSeedPersisterFactoryMock.Setup(m => m.ReleaseService(It.IsAny<IDataSeedPersister>()));

			return new DataSeedRunner(dataSeeds, dataSeedRunDecision, dataSeedPersisterFactoryMock.Object);
        }

        [TestClass]
        public class CanInsertNewItemsWithGeneratedKey
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_CanInsertNewItemsWithKeyGeneration()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
                    dbContext.Database.DropCreate();
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithNullablePropertySeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithNullableProperty>();
                    Assert.AreEqual(3, items.Count());
                }
            }

            private class ItemWithNullablePropertySeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithNullableProperty { NullableValue = 1 },
		                    new ItemWithNullableProperty { NullableValue = 2 },
		                    new ItemWithNullableProperty { NullableValue = 6 })
                        .PairBy(s => s.NullableValue));
                }
            }
        }

        [TestClass]
        public class CanInsertNewItemsWithoutUpdate
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_CanInsertNewItemsWithoutUpdate()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
                    dbContext.Database.DropCreate();
                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithNullableProperty>();

                    // Assert that all items ARE deleted (property is not ignored) in INSERT even if it is set by ExcludeUpdateExpressions
                    Assert.AreEqual(3, items.Where(i => i.NullableValue != null).Count());
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithNullableProperty() { NullableValue = 1, RequiredValue = 1 },
                            new ItemWithNullableProperty() { NullableValue = 2, RequiredValue = 2 },
                            new ItemWithNullableProperty() { NullableValue = 3, RequiredValue = 3 })
                        .PairBy(s => s.RequiredValue)
                        .ExcludeUpdate(s => s.NullableValue));
                }
            }
        }

        [TestClass]
        public class AreExcludedUpdatePropertiesSetByExcludeUpdateExpressions
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_AreExcludedUpdatePropertiesSetByExcludeUpdateExpressions()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
                    dbContext.Database.DropCreate();
                    dbContext.Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
                    dbContext.Add(new ItemWithDeleted() { Symbol = "B", Deleted = null });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that item is NOT updated, because of ExcludeUpdateExpressions
                    Assert.IsTrue(items.All(item => item.Deleted == null));
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(
                            new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
                            new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now })
						.PairBy(s => s.Symbol)
	                    .ExcludeUpdate(s => s.Deleted));
                }
            }
        }

        [TestClass]
        public class AreExcludedUpdatePropertiesSetByWithoutUpdate
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_AreExcludedUpdatePropertiesSetByWithoutUpdate()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
	                dbContext.Database.DropCreate();
	                dbContext.Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
	                dbContext.Add(new ItemWithDeleted() { Symbol = "B", Deleted = null });
	                dbContext.SaveChanges();

					// Arrange
					DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that items ARE NOT updated, because of WithoutUpdate()
                    Assert.AreEqual(true, items.All(item => item.Deleted == null));
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
	                Seed(For(
			                new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
			                new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now })
		                .PairBy(s => s.Symbol)
						.WithoutUpdate());
                }
            }
        }

        [TestClass]
        public class ItemsArePairedAndUpdated
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_ItemsArePairedAndUpdated()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
	                dbContext.Database.DropCreate();
	                dbContext.Add(new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now });
	                dbContext.Add(new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now });
	                dbContext.SaveChanges();

					// Arrange
					DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that ALL items ARE paired and updated
                    Assert.AreEqual(2, items.Count());
                    Assert.IsTrue(items.All(item => item.Deleted == null));
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
	                Seed(For(
			                new ItemWithDeleted() { Symbol = "A", Deleted = null },
			                new ItemWithDeleted() { Symbol = "B", Deleted = null })
		                .PairBy(s => s.Symbol));

				}
			}
        }

        [TestClass]
        public class CanMixAndUpdateNewItems
        {
            [TestMethod]
            public void DataSeedRunner_SeedData_CanMixAndUpdateNewItems()
            {
                using (TestDbContext dbContext = new TestDbContext())
                {
                    dbContext.Database.DropCreate();

                    dbContext.Add(new ItemWithDeleted() { Symbol = "A", Deleted = null });
                    dbContext.Add(new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>().OrderBy(s => s.Id).ToArray();

                    // Assert that first two items are updated and new two items are created
                    Assert.AreEqual(4, items.Length);
	                Assert.AreEqual(true, items.Single(item => item.Symbol == "A").Deleted != null);
                    Assert.AreEqual(true, items.Single(item => item.Symbol == "B").Deleted != null);
                    Assert.AreEqual(true, items.Single(item => item.Symbol == "C").Deleted == null);
                    Assert.AreEqual(true, items.Single(item => item.Symbol == "D").Deleted != null);
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted() { Symbol = "A", Deleted = DateTime.Now },
                        new ItemWithDeleted() { Symbol = "B", Deleted = DateTime.Now },
                        new ItemWithDeleted() { Symbol = "C", Deleted = null },
                        new ItemWithDeleted() { Symbol = "D", Deleted = DateTime.Now })
                        .PairBy(s => s.Symbol));
                }
            }
        }

    }
}
