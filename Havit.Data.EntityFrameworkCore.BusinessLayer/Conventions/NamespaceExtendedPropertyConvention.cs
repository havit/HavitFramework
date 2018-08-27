using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre nastavenie Namespace extended property na všetky entity v modeli. Z namespace triedy sa odstráni názov assembly (Havit.{Projekt}.Model.Common -> Common).
	/// </summary>
    public class NamespaceExtendedPropertyConvention : IModelConvention
	{
		/// <inheritdoc />
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