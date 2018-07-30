using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Business.CodeMigrations.StoredProcedures
{
    public static class StoredProceduresExtensions
    {
        public static void UseStoredProcedures(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<StoredProceduresMigrationsAnnotationProvider>();
            optionsBuilder.Options.GetExtension<CompositeMigrationsSqlGeneratorExtension>().WithGeneratorType<StoredProceduresMigrationsGenerator>();
        }
    }
}