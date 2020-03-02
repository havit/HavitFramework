using System.Data.SqlClient;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure
{
    [TestClass]
    public class CompositeMigrationsSqlGeneratorExtensionTests
    {
        /// <summary>
        /// Tests whether resolving IMigrationOperationSqlGenerator from two DbContext instances return two distinct instances of IMigrationOperationSqlGenerator.
        /// </summary>
        [TestMethod]
        public void CompositeMigrationsSqlGeneratorExtension_ResolvingIMigrationsSqlGeneratorTwice_ResolvedInstancesAreNotSame()
        {
            using (var dbContext1 = new TestDbContext())
            using (var dbContext2 = new TestDbContext())
            {
                var instance1 = dbContext1.GetService<IMigrationsSqlGenerator>();
                var instance2 = dbContext2.GetService<IMigrationsSqlGenerator>();
                Assert.AreNotSame(instance1, instance2);
            }
        }

        /// <summary>
        /// Tests whether resolved IMigrationOperationSqlGenerator have access to ICurrentDbContext, i.e. they're resolved in scope of current DbContext.
        /// </summary>
        [TestMethod]
        public void CompositeMigrationsSqlGeneratorExtension_ResolveMigrationOperationSqlGenerator_ResolvedMigrationOperationSqlGeneratorCanAccessDbContext()
        {
            using (var dbContext = new TestDbContext())
            {
                var composite = dbContext.GetService<IMigrationOperationSqlGenerator>();

                Assert.IsInstanceOfType(composite, typeof(FakeMigrationOperationSqlGenerator));

                Assert.IsNotNull(((FakeMigrationOperationSqlGenerator)composite).CurrentDbContext);
            }
        }

        private class TestDbContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));
                optionsBuilder.UseExtendedMigrationsInfrastructure();

                IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

                builder.AddOrUpdateExtension(optionsBuilder.Options
                    .FindExtension<CompositeMigrationsSqlGeneratorExtension>()
                    .WithGeneratorType<FakeMigrationOperationSqlGenerator>());
            }
        }

        private class FakeMigrationOperationSqlGenerator : MigrationOperationSqlGenerator
        {
            public ICurrentDbContext CurrentDbContext { get; }

            public FakeMigrationOperationSqlGenerator(ICurrentDbContext currentDbContext)
            {
                CurrentDbContext = currentDbContext;
            }
        }
    }
}