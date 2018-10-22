using System;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre názvy stĺpcov s primárnym kľúčom. Pre normálne tabuľky nastaví názov PK na {TableName}ID a pre väzobné M:N len zmení suffix z Id na ID.
	/// </summary>
	public class PrefixedTablePrimaryKeysConvention : IModelConvention
	{
		private readonly string tableSuffix;

		/// <summary>
		/// Konštruktor. Parametrom <see cref="tableSuffix"/> je možné zmeniť suffix stĺpca s primárnym kľúčom.
		/// </summary>
		public PrefixedTablePrimaryKeysConvention(string tableSuffix = "ID")
		{
			this.tableSuffix = tableSuffix;
		}

		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType entityType in modelBuilder.Model.GetApplicationEntityTypes())
			{
				IMutableKey primaryKey = entityType.FindPrimaryKey();
				if (primaryKey != null)
				{
					foreach (IMutableProperty property in primaryKey.Properties)
					{
						string columnName = property.Relational().ColumnName;
						if (columnName.Equals("Id", StringComparison.OrdinalIgnoreCase))
						{
							columnName = $"{entityType.ShortName()}{tableSuffix}";
						}
						else if (columnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
						{
							columnName = columnName.Substring(0, columnName.Length - 2) + tableSuffix;
						}
						property.Relational().ColumnName = columnName;
					}
				}
			}
		}
	}
}