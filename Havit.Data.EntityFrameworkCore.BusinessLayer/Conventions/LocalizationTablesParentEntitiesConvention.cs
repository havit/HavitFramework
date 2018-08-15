using System.Linq;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	public class LocalizationTablesParentEntitiesConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			var localizationTables = modelBuilder.Model.GetEntityTypes().Where(t => t.Name.EndsWith("Localization") && (t.Name.Length > "Localization".Length));
			foreach (IMutableEntityType entityType in localizationTables)
			{
				IMutableProperty parentIdProperty = entityType.GetProperties().FirstOrDefault(prop => prop.Name == "ParentId");
				if (parentIdProperty != null)
				{
					IMutableEntityType principalEntityType = parentIdProperty.GetContainingForeignKeys().FirstOrDefault().PrincipalEntityType;
					string pkColumnName = principalEntityType.FindPrimaryKey().Properties.First().Relational().ColumnName;
					parentIdProperty.Relational().ColumnName = pkColumnName;
				}
			}
		}
	}
}