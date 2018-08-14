using System.Reflection;
using Havit.Business.CodeMigrations.Conventions;
using Havit.Business.CodeMigrations.DbInjections;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DbContext = Havit.Data.Entity.DbContext;

namespace Havit.Business.CodeMigrations
{
    public class BusinessLayerDbContext : DbContext
	{
	    public BusinessLayerDbContext()
		{
		}

		public BusinessLayerDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseCodeMigrationsInfrastructure();
			optionsBuilder.UseDbInjections();
			optionsBuilder.UseSqlServerExtendedProperties();
		}

		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.ForSqlServerExtendedPropertiesAttributes();

			ApplyConventions(modelBuilder);
		}

		protected virtual void ApplyConventions(ModelBuilder modelBuilder)
		{
			// TODO JK: Refactoring do IModelConvention + přesunout GetModelConventions
			modelBuilder.ApplyPrefixedTablePrimaryKeys();
			modelBuilder.ApplyForeignKeysColumnNames();
			modelBuilder.ApplyLocalizationTablesParentEntities();
			modelBuilder.ApplyDefaultsForStrings();
			modelBuilder.ApplyDefaultNamespaces();
			modelBuilder.ApplyCollectionExtendedProperties();
			modelBuilder.UseXmlCommentsForDescriptionProperty();
            modelBuilder.UseSequencesForEnumClasses();
            modelBuilder.ApplyBusinessLayerIndexes();
        }

	    protected void RegisterDbInjections(ModelBuilder modelBuilder, Assembly injectionsAssembly = default)
	    {
            modelBuilder.ForDbInjections(this.GetService<IDbInjectionAnnotationProvider>(), injectionsAssembly ?? Assembly.GetCallingAssembly());
	    }
	}
}
