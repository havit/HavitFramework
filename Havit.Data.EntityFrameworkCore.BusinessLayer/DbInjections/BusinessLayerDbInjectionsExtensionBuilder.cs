using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class BusinessLayerDbInjectionsExtensionBuilder : DbInjectionsExtensionBuilderBase
    {
        public BusinessLayerDbInjectionsExtensionBuilder(DbInjectionsExtensionBuilder builder)
            : base(builder)
        {
        }

        public DbInjectionsExtensionBuilder UseExtendedProperties() =>
            WithOption(e => e.WithAnnotationProvider<ExtendedPropertiesAnnotationProvider>());

        public DbInjectionsExtensionBuilder UseBusinessLayerStoredProcedures() =>
            WithOption(e => e
                .WithAnnotationProvider<StoredProcedureAttachPropertyAnnotationProvider>()
                .WithAnnotationProvider<StoredProcedureMsDescriptionPropertyAnnotationProvider>());
    }
}