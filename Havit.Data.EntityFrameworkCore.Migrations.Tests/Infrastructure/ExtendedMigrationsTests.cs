using System;
using System.Collections.Immutable;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure
{
    /// <summary>
    /// Tests for extended migrations infrastructure.
    /// </summary>
    [TestClass]
    public class ExtendedMigrationsTests
    {
        /// <summary>
        /// Tests, whether multiple invocations of <see cref="InfrastructureExtensions.UseExtendedMigrationsInfrastructure"/> 
        /// does not mess up previously registered instances of <see cref="IMigrationOperationSqlGenerator"/>.
        /// </summary>
        [TestMethod]
        public void InfrastructureExtensions_RegisteringInfrastructureMultipleTimes_ExtensionContainsAllMigrationOperationGenerators()
        {
            static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseExtendedMigrationsInfrastructure();

                IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

                builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
                    .WithGeneratorType<FirstMigrationOperationSqlGenerator>());

                optionsBuilder.UseExtendedMigrationsInfrastructure();

                builder.AddOrUpdateExtension(optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>()
                    .WithGeneratorType<SecondMigrationOperationSqlGenerator>());
            }

            using (var dbContext = new TestDbContext(OnConfiguring))
            {
                _ = dbContext.Model;

                var generatorTypes = dbContext.CompositeMigrationsSqlGeneratorExtension.GeneratorTypes.ToArray();

                Assert.AreEqual(2, generatorTypes.Length);

                CollectionAssert.Contains(generatorTypes, typeof(FirstMigrationOperationSqlGenerator));
                CollectionAssert.Contains(generatorTypes, typeof(SecondMigrationOperationSqlGenerator));
            }
        }

        private class TestDbContext : DbContext
        {
            private readonly Action<DbContextOptionsBuilder> onConfiguring;

            public CompositeMigrationsSqlGeneratorExtension CompositeMigrationsSqlGeneratorExtension { get; private set; }

            public TestDbContext(Action<DbContextOptionsBuilder> onConfiguring = default)
            {
                this.onConfiguring = onConfiguring;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseSqlServer("Server=Dummy");

                onConfiguring?.Invoke(optionsBuilder);

                CompositeMigrationsSqlGeneratorExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>();
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class FirstMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class SecondMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
        {
        }
    }
}