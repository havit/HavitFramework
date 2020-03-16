using System;
using System.Data.SqlClient;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure
{
    /// <summary>
    /// Tests involving <see cref="CompositeMigrationsAnnotationProviderExtension"/>, e.g. registration of required services, resolving etc.
    /// </summary>
    [TestClass]
    public class CompositeMigrationsAnnotationProviderExtensionTests
    {
        /// <summary>
        /// Tests whether registering same <see cref="IMigrationsAnnotationProvider"/> into <see cref="CompositeMigrationsAnnotationProvider"/>
        /// does not produce duplicate types.
        /// </summary>
        [TestMethod]
        public void CompositeMigrationsAnnotationProvider_RegisterSameProviderTwice_ProviderIsRegisteredOnlyOnce()
        {
            static void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseExtendedMigrationsInfrastructure();

                IDbContextOptionsBuilderInfrastructure builder = optionsBuilder;

                builder.AddOrUpdateExtension(optionsBuilder.Options
                    .FindExtension<CompositeMigrationsAnnotationProviderExtension>()
                    .WithAnnotationProvider<FakeMigrationsAnnotationProvider>()
                    .WithAnnotationProvider<FakeMigrationsAnnotationProvider>());
            }

            using (var dbContext = new TestDbContext(OnConfiguring))
            {
                _ = dbContext.Model;

                Assert.AreEqual(1, dbContext.CompositeMigrationsAnnotationProviderExtension.Providers.Count);

                Assert.AreSame(dbContext.CompositeMigrationsAnnotationProviderExtension.Providers.First(), typeof(FakeMigrationsAnnotationProvider));
            }
        }

        private class TestDbContext : DbContext
        {
            private readonly Action<DbContextOptionsBuilder> onConfiguring;

            public CompositeMigrationsAnnotationProviderExtension CompositeMigrationsAnnotationProviderExtension { get; private set; }

            public TestDbContext(Action<DbContextOptionsBuilder> onConfiguring = default)
            {
                this.onConfiguring = onConfiguring;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(new SqlConnection("Database=Dummy"));

                onConfiguring?.Invoke(optionsBuilder);

                CompositeMigrationsAnnotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>();
            }
        }

        private class FakeMigrationsAnnotationProvider : MigrationsAnnotationProvider
        {
            public FakeMigrationsAnnotationProvider(MigrationsAnnotationProviderDependencies dependencies)
                : base(dependencies)
            {
            }
        }
    }
}