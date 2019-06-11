using System;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    /// <summary>
    /// Extension metódy pre konfiguráciu DB Injections v kombinácii s Business Layerom.
    /// </summary>
    public static class BusinessLayerDbInjectionsExtensions
    {
        public static DbInjectionsExtensionBuilder UseExtendedProperties(this DbInjectionsExtensionBuilder optionsBuilder)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);

            // TODO: fix fluent API
            new BusinessLayerDbInjectionsExtensionBuilder(((IDbInjectionsExtensionBuilderInfrastructure)optionsBuilder).OptionsBuilder).UseExtendedProperties();
            return optionsBuilder;
        }

        public static DbInjectionsExtensionBuilder UseBusinessLayerStoredProcedures(this DbInjectionsExtensionBuilder optionsBuilder)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);

            // TODO: fix fluent API
            new BusinessLayerDbInjectionsExtensionBuilder(((IDbInjectionsExtensionBuilderInfrastructure)optionsBuilder).OptionsBuilder).UseBusinessLayerStoredProcedures();
            return optionsBuilder;
        }
    }
}