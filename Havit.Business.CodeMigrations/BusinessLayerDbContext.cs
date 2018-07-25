using System.Reflection;
using Havit.Business.CodeMigrations.Conventions;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.Infrastructure;
using Havit.Business.CodeMigrations.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.SqlServer.Migrations.Internal;
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
			optionsBuilder.UseStoredProcedures();
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

		protected virtual void AddStoredProceduresAnnotations(ModelBuilder modelBuilder, Assembly entityAssembly = null)
		{
			StoredProceduresAnnotationsHelper.AddStoredProcedureAnnotations(modelBuilder.Model, entityAssembly ?? GetType().Assembly);
		}
	}
}
