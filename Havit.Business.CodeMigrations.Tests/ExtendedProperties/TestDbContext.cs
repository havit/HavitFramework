using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Havit.Business.CodeMigrations.Tests.ExtendedProperties
{
	internal class TestDbContext : BusinessLayerDbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer(new SqlConnection("Database=DummyDatabase"));
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			NegateParentRegularTablePrimaryKeys(modelBuilder);
		}

		private static void NegateParentRegularTablePrimaryKeys(ModelBuilder modelBuilder)
		{
			var tables = modelBuilder.Model.GetEntityTypes().Where(entityType => entityType.FindPrimaryKey()?.Properties.Count == 1);
			foreach (var table in tables)
			{
				var primaryKeyProperty = table.FindPrimaryKey().Properties[0];
				if (primaryKeyProperty.PropertyInfo.Name == "Id")
				{
					primaryKeyProperty.Relational().ColumnName = "Id";
				}
			}
		}
	}
}
