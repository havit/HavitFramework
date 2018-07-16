using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DbContext = Havit.Data.Entity.DbContext;

namespace Havit.Business.CodeMigrations
{
	public class BusinessLayerDbContext : DbContext
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			ApplyConventions(modelBuilder);
		}

		private void ApplyConventions(ModelBuilder modelBuilder)
		{
			LocalizationTables(modelBuilder);
			RegularTablePrimaryKeys(modelBuilder);
			DefaultsForStrings(modelBuilder);
		}

		private void RegularTablePrimaryKeys(ModelBuilder modelBuilder, string tableSuffix = "ID")
		{
			var tables = modelBuilder.Model.GetEntityTypes().Where(entityType => entityType.FindPrimaryKey()?.Properties.Count == 1);
			foreach (IMutableEntityType table in tables)
			{
				IMutableProperty primaryKeyProperty = table.FindPrimaryKey().Properties[0];
				if (primaryKeyProperty.Relational().ColumnName == "Id")
				{
					primaryKeyProperty.Relational().ColumnName = $"{table.ShortName()}{tableSuffix}";
				}
			}
		}

		private static void LocalizationTables(ModelBuilder modelBuilder)
		{
			var localizationTables = modelBuilder.Model.GetEntityTypes().Where(t => t.Name.EndsWith("Localization") && (t.Name.Length > "Localization".Length));
			foreach (IMutableEntityType entityType in localizationTables)
			{
				IMutableProperty parentIdProperty = entityType.GetProperties().FirstOrDefault(prop => prop.Name == "ParentId");
				if (parentIdProperty != null)
				{
					IMutableEntityType principalEntityType = parentIdProperty.GetContainingForeignKeys().FirstOrDefault().PrincipalEntityType;
					parentIdProperty.Relational().ColumnName = $"{principalEntityType.Name}Id";
				}
			}
		}

		private void DefaultsForStrings(ModelBuilder modelBuilder)
		{
			var stringProperties = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetDeclaredProperties().Where(prop => prop.ClrType == typeof(string)));
			foreach (IMutableProperty property in stringProperties)
			{
				if ((property.Relational().DefaultValue == null) && string.IsNullOrEmpty(property.Relational().DefaultValueSql))
				{
					property.Relational().DefaultValue = "";
				}
			}
		}
	}
}
