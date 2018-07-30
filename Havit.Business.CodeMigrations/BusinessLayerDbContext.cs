using System.Reflection;
using Havit.Business.CodeMigrations.Conventions;
using Havit.Business.CodeMigrations.DbInjections;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
			optionsBuilder.UseSqlServerExtendedProperties();
			optionsBuilder.UseDbInjections();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ApplyConventions(modelBuilder);

			modelBuilder.ForSqlServerExtendedPropertiesAttributes();
		}

		protected virtual void ApplyConventions(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyPrefixedTablePrimaryKeys();
			modelBuilder.ApplyLocalizationTablesParentEntities();
			modelBuilder.ApplyDefaultsForStrings();
			modelBuilder.ApplyDefaultNamespaces();
			modelBuilder.ApplyCollectionExtendedProperties();
		}

	    protected void RegisterDbInjections(ModelBuilder modelBuilder, Assembly injectionsAssembly = default)
	    {
            modelBuilder.ForDbInjections(injectionsAssembly ?? Assembly.GetCallingAssembly());
	    }
	}
}
