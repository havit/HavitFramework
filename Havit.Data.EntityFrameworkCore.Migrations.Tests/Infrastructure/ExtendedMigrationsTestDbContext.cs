using System;
using Havit.Data.EntityFrameworkCore.Migrations.Infrastructure.ModelExtensions;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.Infrastructure
{
    /// <summary>
    /// <see cref="DbContext"/> used in tests for various parts of extended migrations infrastructure.
    /// </summary>
    internal class ExtendedMigrationsTestDbContext : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> onConfiguring;

        /// <summary>
        /// Gets <see cref="CompositeMigrationsSqlGeneratorExtension"/> for assertions in tests.
        /// </summary>
        public CompositeMigrationsSqlGeneratorExtension CompositeMigrationsSqlGeneratorExtension { get; private set; }

        /// <summary>
        /// Gets <see cref="CompositeMigrationsAnnotationProviderExtension"/> for assertions in tests.
        /// </summary>
        public CompositeMigrationsAnnotationProviderExtension CompositeMigrationsAnnotationProviderExtension { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExtendedMigrationsTestDbContext(Action<DbContextOptionsBuilder> onConfiguring = default)
        {
            this.onConfiguring = onConfiguring;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.EnableServiceProviderCaching(false);
            optionsBuilder.UseSqlServer("Server=Dummy");

            onConfiguring?.Invoke(optionsBuilder);

            CompositeMigrationsSqlGeneratorExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsSqlGeneratorExtension>();
            CompositeMigrationsAnnotationProviderExtension = optionsBuilder.Options.FindExtension<CompositeMigrationsAnnotationProviderExtension>();
        }
    }
}