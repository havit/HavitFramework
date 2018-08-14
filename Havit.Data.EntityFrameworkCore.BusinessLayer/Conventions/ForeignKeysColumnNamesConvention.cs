using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
    public static class ForeignKeysColumnNamesConvention
    {
        public static void ApplyForeignKeysColumnNames(this ModelBuilder modelBuilder, string fkColumnSuffix = "ID")
        {
            Contract.Requires<ArgumentNullException>(modelBuilder != null);

	        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
	        {
		        IEnumerable<IMutableProperty> foreignKeyProperties = entityType.GetForeignKeys()
			        .SelectMany(fk => fk.Properties)
			        .Except(entityType.FindPrimaryKey().Properties);

				foreach (IMutableProperty property in foreignKeyProperties)
				{
					if (property.Relational().ColumnName.EndsWith("Id"))
					{
						property.Relational().ColumnName = property.Relational().ColumnName.Substring(0, property.Relational().ColumnName.Length - 2) + fkColumnSuffix;
					}
				}
            }
        }
    }
}