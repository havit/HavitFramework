using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Business.CodeMigrations.Conventions
{
	public class DefaultsForStringsConvention
	{
		public static void Apply(ModelBuilder modelBuilder)
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