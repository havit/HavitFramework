using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.StoredProcedures
{
    public static class StoredProceduresExtensions
    {
        public static void UseStoredProcedures(this DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.Options.GetExtension<CompositeMigrationsAnnotationProviderExtension>().WithAnnotationProvider<StoredProceduresMigrationsAnnotationProvider>();
            optionsBuilder.Options.GetExtension<CompositeMigrationsSqlGeneratorExtension>().WithGeneratorType<StoredProceduresMigrationOperationSqlGenerator>();
        }
    }
}