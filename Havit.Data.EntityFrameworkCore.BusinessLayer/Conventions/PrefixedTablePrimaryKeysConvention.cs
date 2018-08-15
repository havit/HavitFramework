using System;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class PrefixedTablePrimaryKeysConvention : IModelConvention
	{
		private readonly string tableSuffix;

		public PrefixedTablePrimaryKeysConvention(string tableSuffix = "ID")
		{
			this.tableSuffix = tableSuffix;
		}

		public void Apply(ModelBuilder modelBuilder)
		{
			foreach (IMutableEntityType table in modelBuilder.Model.GetEntityTypes())
			{
				IMutableKey primaryKey = table.FindPrimaryKey();
				foreach (IMutableProperty property in primaryKey.Properties)
				{
					string columnName = property.Relational().ColumnName;
					if (columnName.Equals("Id", StringComparison.OrdinalIgnoreCase))
					{
						columnName = $"{table.ShortName()}{tableSuffix}";
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