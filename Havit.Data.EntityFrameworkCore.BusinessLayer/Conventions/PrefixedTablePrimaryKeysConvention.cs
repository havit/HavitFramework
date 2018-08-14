using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public static class PrefixedTablePrimaryKeysConvention
	{
		public static void ApplyPrefixedTablePrimaryKeys(this ModelBuilder modelBuilder, string tableSuffix = "ID")
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
	}
}