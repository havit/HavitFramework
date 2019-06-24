using System;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.DbInjections;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
    /// <summary>
    /// Extension metódy pre konfiguráciu DB Injections v kombinácii s Business Layerom.
    /// </summary>
    public static class BusinessLayerDbInjectionsExtensions
    {
        /// <summary>
        /// Zapne podporu pre extended properties na <see cref="IDbInjector"/> objektoch. Umožňuje automaticky spravovať extended properties pre objekty DbInjections pomocou migrácii.
        /// </summary>
        /// <remarks>
        /// Je nutné odekorovať <see cref="IDbInjector"/> objekty pomocou atribútov dediacich z <see cref="DbInjectionExtendedPropertiesAttribute"/>. Podporované sú len tie objekty v DB, na ktoré je možné pridať extended properties v SQL Serveri.
        /// </remarks>
        /// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
        public static DbInjectionsExtensionBuilder UseExtendedProperties(this DbInjectionsExtensionBuilder optionsBuilder)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);

            return new BusinessLayerDbInjectionsExtensionBuilder(optionsBuilder)
                .UseExtendedProperties();
        }

        /// <summary>
        /// Zapne podporu pre rozšírenia uložených procedúr podporovaných Business Layerom (resp. jeho generátorom). Jedná sa primárne o (polo)automatické generovanie extended properties pre uložené procedúry ako napr. MS_Description (z XML komentáru) alebo Attach (z Attach atribútu).
        /// </summary>
        /// <remarks>
        /// Pre podporu nastavenia MS_Description extended property je nutné zapnúť generovanie dokumentačného XML súboru z XML komentárov na projekte, kde sa DbInjectory uložených procedúr nachádzajú.
        /// </remarks>
        /// <returns>Inštancia <see cref="DbInjectionsExtensionBuilder"/>, kvôli implementácii Fluent API.</returns>
        public static DbInjectionsExtensionBuilder UseBusinessLayerStoredProcedures(this DbInjectionsExtensionBuilder optionsBuilder)
        {
            Contract.Requires<ArgumentNullException>(optionsBuilder != null);

            return new BusinessLayerDbInjectionsExtensionBuilder(optionsBuilder)
                .UseBusinessLayerStoredProcedures();
        }
    }
}