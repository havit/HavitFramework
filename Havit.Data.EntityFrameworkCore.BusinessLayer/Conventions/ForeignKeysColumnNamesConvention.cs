using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Diagnostics.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre názvy stĺpcov s cudzím kľúčom - mení suffix z Id na ID (konvencia BusinessLayeru).
	/// </summary>
    public class ForeignKeysColumnNamesConvention : IModelConvention
    {
	    private readonly string fkColumnSuffix;

		/// <summary>
		/// Konštruktor. Parametrom <see cref="fkColumnSuffix"/> je možné prenastaviť nový suffix stĺpca s cudzím kľúčom.
		/// </summary>
		/// <param name="fkColumnSuffix"></param>
	    public ForeignKeysColumnNamesConvention(string fkColumnSuffix = "ID")
	    {
		    this.fkColumnSuffix = fkColumnSuffix;
	    }

	    /// <inheritdoc />
	    public void Apply(ModelBuilder modelBuilder)
        {
            Contract.Requires<ArgumentNullException>(modelBuilder != null);

	        foreach (IMutableEntityType entityType in modelBuilder.Model.GetApplicationEntityTypes())
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