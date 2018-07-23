using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.ExtendedProperties.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.Conventions
{
	public static class NamespaceExtendedPropertyConvention
	{
		public static void Apply(ModelBuilder modelBuilder)
		{
			var tables = modelBuilder.Model.GetEntityTypes();
			foreach (IMutableEntityType table in tables)
			{
				if (table.ClrType.GetCustomAttributes<NamespaceAttribute>().Any())
				{
					continue;
				}
				table.AddExtendedProperties(new Dictionary<string, string>()
				{
					{ NamespaceAttribute.PropertyName, table.ClrType.Namespace },
				});
			}
		}
	}
}