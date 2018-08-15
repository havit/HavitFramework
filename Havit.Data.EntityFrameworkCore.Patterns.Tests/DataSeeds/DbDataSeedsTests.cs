using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds
{
    public class DbDataSeedsTests
    {
        private static DataSeedRunner GetDefaultDataSeedRunner(IDbContext dbContext, params IDataSeed[] dataSeedsParams)
        {
            IEnumerable<IDataSeed> dataSeeds = new List<IDataSeed>(dataSeedsParams);
            IDataSeedRunDecision dataSeedRunDecision = new AlwaysRunDecision();
            IDataSeedPersister dataSeedPersister = new DbDataSeedPersister(dbContext);

            return new DataSeedRunner(dataSeeds, dataSeedRunDecision, dataSeedPersister);
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
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();
                    Assert.AreEqual(3, items.Count());
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted(), new ItemWithDeleted(), new ItemWithDeleted())
                        .PairBy(s => s.Id));
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

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that all items ARE deleted (property is not ignored) in INSERT even if it is set by ExcludeUpdateExpressions
                    Assert.AreEqual(3, items.Where(i => i.Deleted != null).Count());
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted() { Deleted = DateTime.Now },
                            new ItemWithDeleted() { Deleted = DateTime.Now.AddDays(-1) },
                            new ItemWithDeleted() { Deleted = DateTime.Now.AddHours(-1) })
                        .PairBy(s => s.Id)
                        .ExcludeUpdate(s => s.Deleted));
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
                    dbContext.Add(new ItemWithDeleted() { Id = 1, Deleted = null });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that item is NOT updated, because of ExcludeUpdateExpressions
                    Assert.AreEqual(true, items.Single().Deleted == null);
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    IDataSeedFor<ItemWithDeleted> dataSeed = For(
                            new ItemWithDeleted() { Id = 1, Deleted = DateTime.Now })
                        .PairBy(s => s.Id);
                    dataSeed.Configuration.ExcludeUpdateExpressions = new List<Expression<Func<ItemWithDeleted, object>>>()
                    {
                        (entity) => entity.Deleted
                    };

                    Seed(dataSeed);
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
                    dbContext.Add(new ItemWithDeleted() { Id = 1, Deleted = null });
                    dbContext.Add(new ItemWithDeleted() { Id = 2, Deleted = null });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that items ARE NOT updated, because of WithoutUpdate()
                    Assert.AreEqual(2, items.Count(i => i.Deleted == null));
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted() { Id = 1, Deleted = DateTime.Now }, new ItemWithDeleted() { Id = 2, Deleted = DateTime.Now })
                        .PairBy(s => s.Id).WithoutUpdate());
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
                    dbContext.Add(new ItemWithDeleted() { Id = 1, Deleted = null });
                    dbContext.Add(new ItemWithDeleted() { Id = 2, Deleted = null });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>();

                    // Assert that ALL items ARE paired and updated
                    Assert.AreEqual(2, items.Count());
                    Assert.AreEqual(2, items.Count(i => i.Deleted != null));
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted() { Id = 1, Deleted = DateTime.Now }, new ItemWithDeleted() { Id = 2, Deleted = DateTime.Now })
                        .PairBy(s => s.Id));
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

                    dbContext.Add(new ItemWithDeleted() { Id = 1, Deleted = null });
                    dbContext.Add(new ItemWithDeleted() { Id = 2, Deleted = DateTime.Now });
                    dbContext.SaveChanges();

                    // Arrange
                    DataSeedRunner dataSeedRunner = GetDefaultDataSeedRunner(dbContext, new ItemWithDeletedSeed());

                    dataSeedRunner.SeedData<DefaultProfile>();

                    var items = dbContext.Set<ItemWithDeleted>().OrderBy(s => s.Id).ToArray();

                    // Assert that first two items are updated and new two items are created
                    Assert.AreEqual(4, items.Length);
                    Assert.AreEqual(true, items[0].Deleted != null);
                    Assert.AreEqual(true, items[1].Deleted != null);
                    Assert.AreEqual(true, items[2].Deleted == null);
                    Assert.AreEqual(true, items[3].Deleted != null);
                }
            }

            private class ItemWithDeletedSeed : DataSeed<DefaultProfile>
            {
                public override void SeedData()
                {
                    Seed(For(new ItemWithDeleted() { Id = 1, Deleted = DateTime.Now },
                        new ItemWithDeleted() { Id = 2, Deleted = DateTime.Now },
                        new ItemWithDeleted() { Id = 3, Deleted = null },
                        new ItemWithDeleted() { Id = 4, Deleted = DateTime.Now })
                        .PairBy(s => s.Id));
                }
            }
        }

    }
}
