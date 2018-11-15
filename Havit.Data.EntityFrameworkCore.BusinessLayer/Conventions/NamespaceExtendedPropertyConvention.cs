using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
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
			foreach (IMutableEntityType entityType in modelBuilder.Model.GetApplicationEntityTypes(includeManyToManyEntities: false))
			{
				string entityNamespace = entityType.ClrType.Namespace?.Replace(entityType.ClrType.Assembly.GetName().Name, "").Trim('.');
				if (!String.IsNullOrEmpty(entityNamespace))
				{
					entityType.AddExtendedProperties(new Dictionary<string, string>()
					{
						{ "Namespace", entityNamespace },
					});
				}
			}
		}
	}
}