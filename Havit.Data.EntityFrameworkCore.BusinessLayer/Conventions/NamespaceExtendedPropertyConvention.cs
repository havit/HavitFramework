using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
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
				// pokud jde o many to many tabulku, která má další ignorované sloupce, nebudeme extended property generovat
				var notIgnoredProperties = entityType.GetProperties()
					.Where(property => !property.IsShadowProperty &&
					                   !property.PropertyInfo.GetCustomAttributes(typeof(IgnoredAttribute), false).Any())
					.ToList();
				if ((notIgnoredProperties.Count == 2) &&
				    notIgnoredProperties.All(property => property.IsPrimaryKey()) &&
				    notIgnoredProperties.All(property => property.IsForeignKey()))
				{
					continue;
				}

				// pokud jde o lokalizační tabulku, která je ve stejném namespaces s lokalizovanou tabulkou, nebudeme extended property generovat
				var localizationInterfaceType = entityType.ClrType.GetInterfaces()
					.FirstOrDefault(iType =>
						iType.IsGenericType &&
						(iType.GetGenericTypeDefinition() == typeof(Havit.Model.Localizations.ILocalization<,>)));
				if (localizationInterfaceType != null)
				{
					Type localizedType = localizationInterfaceType.GetGenericArguments().First(); // TLocalizedEntity
					if (localizedType.Namespace == entityType.ClrType.Namespace) // namespace lokalizované a této lokalizační třídy je stejný
					{
						continue; // nebudeme extended property generovat
					}
				}

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