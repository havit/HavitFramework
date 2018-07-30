using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public static class DbInjectionsExtensions
    {
        public static void UseDbInjections(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<DbInjectionsAnnotationProvider>();
            optionsBuilder.Options.GetExtension<CompositeMigrationsSqlGeneratorExtension>().WithGeneratorType<DbInjectionMigrationsGenerator>();
        }
    }
}