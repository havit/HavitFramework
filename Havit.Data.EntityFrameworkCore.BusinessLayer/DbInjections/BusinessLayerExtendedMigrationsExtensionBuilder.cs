using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    public class BusinessLayerExtendedMigrationsExtensionBuilder : ExtendedMigrationsExtensionBuilderBase
    {
        public BusinessLayerExtendedMigrationsExtensionBuilder(ExtendedMigrationsExtensionBuilder builder)
            : base(builder)
        {
        }

        public ExtendedMigrationsExtensionBuilder UseExtendedProperties() =>
            WithOption(e => e.WithAnnotationProvider<ExtendedPropertiesAnnotationProvider>());

        public ExtendedMigrationsExtensionBuilder UseBusinessLayerStoredProcedures() =>
            WithOption(e => e
                .WithAnnotationProvider<StoredProcedureAttachPropertyAnnotationProvider>()
                .WithAnnotationProvider<StoredProcedureMsDescriptionPropertyAnnotationProvider>());
    }
}