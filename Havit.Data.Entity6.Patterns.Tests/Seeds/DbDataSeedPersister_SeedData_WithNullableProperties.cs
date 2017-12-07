﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Patterns.DataSeeds;
using Havit.Data.Entity.Patterns.Tests.Infrastructure;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Patterns.Tests.Seeds
{
    [TestClass]
    public class DbDataSeedPersister_SeedData_WithNullableProperties
    {
        /// <summary>
        /// Reprodukce bugu 35740
        /// </summary>
        [TestMethod]
        public void DbDataSeedPersister_SeedData_SupportsPairingByNullableProperties()
        {
            // Arrange
            DataSeedRunner dataSeedRunner = new DataSeedRunner(new IDataSeed[] { new ItemWithNullablePropertySeed() }, new AlwaysRunDecision(), new DbDataSeedPersister(new TestDbContext()));

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
                Seed(For(new Tests.ItemWithNullableProperty { NullableValue = 1 }).PairBy(item => item.NullableValue));
            }
        }

    }
}