using System.Collections.Generic;
using Havit.Data.Entity.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
    public class NamespaceExtendedPropertyConvention : IModelConvention
	{
		public void Apply(ModelBuilder modelBuilder)
		{
			var tables = modelBuilder.Model.GetEntityTypes();
			foreach (IMutableEntityType table in tables)
			{
				string entityNamespace = table.ClrType.Namespace?.Replace(table.ClrType.Assembly.GetName().Name, "").Trim('.');
				table.AddExtendedProperties(new Dictionary<string, string>()
				{
					{ "Namespace", entityNamespace },
				});
			}
		}
	}
}