using System.Linq;
using Havit.Data.Entity.Conventions;
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