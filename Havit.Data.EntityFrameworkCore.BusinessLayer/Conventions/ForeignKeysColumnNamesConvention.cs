using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Entity.Conventions;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
    public class ForeignKeysColumnNamesConvention : IModelConvention
    {
	    private readonly string fkColumnSuffix;

	    public ForeignKeysColumnNamesConvention(string fkColumnSuffix = "ID")
	    {
		    this.fkColumnSuffix = fkColumnSuffix;
	    }

        public void Apply(ModelBuilder modelBuilder)
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