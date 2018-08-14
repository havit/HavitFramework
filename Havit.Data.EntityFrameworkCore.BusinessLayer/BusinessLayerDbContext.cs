using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DbContext = Havit.Data.Entity.DbContext;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
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
