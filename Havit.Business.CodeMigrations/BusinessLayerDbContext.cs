using System.Linq;
using System.Reflection;
using Havit.Business.CodeMigrations.Conventions;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
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
			
			optionsBuilder.UseSqlServerExtendedProperties();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ApplyConventions(modelBuilder);

			modelBuilder.ForSqlServerExtendedProperties();
		}

		private void ApplyConventions(ModelBuilder modelBuilder)
		{
			LocalizationTablesConvention.Apply(modelBuilder);
			RegularTablePrimaryKeysConvention.Apply(modelBuilder);
			DefaultsForStringsConvention.Apply(modelBuilder);
		}
	}
}
