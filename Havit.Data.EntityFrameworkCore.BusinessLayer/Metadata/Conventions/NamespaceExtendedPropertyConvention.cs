using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions
{
	/// <summary>
	/// Konvencia pre nastavenie Namespace extended property na všetky entity v modeli. Z namespace triedy sa odstráni názov assembly (Havit.{Projekt}.Model.Common -> Common).
	/// </summary>
	public class NamespaceExtendedPropertyConvention : IEntityTypeAddedConvention
	{
		/// <inheritdoc />
		public void ProcessEntityTypeAdded(IConventionEntityTypeBuilder entityTypeBuilder, IConventionContext<IConventionEntityTypeBuilder> context)
		{
			IConventionEntityType entityType = entityTypeBuilder.Metadata;

			// Systémové tabulky nechceme změnit.
			if (entityType.IsSystemType())
			{
				return;
			}

			if (entityType.IsConventionSuppressed<NamespaceExtendedPropertyConvention>())
			{
				return;
			}

			// pokud jde o many to many tabulku, nebudeme extended property generovat
			// zde řešíme i nestandardní many-to-many tabulku s dalšími (ignorovanými) sloupci
			if (entityTypeBuilder.Metadata.IsBusinessLayerManyToManyEntity())
			{
				return; // nebudeme extended property generovat
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
					return; // nebudeme extended property generovat
				}
			}

			string entityNamespace = entityType.ClrType.Namespace?.Replace(entityType.ClrType.Assembly.GetName().Name, "").Trim('.');
			if (!String.IsNullOrEmpty(entityNamespace))
			{
				entityTypeBuilder.Metadata.AddExtendedProperties(new Dictionary<string, string>()
					{
						{ "Namespace", entityNamespace },
					}, 
					fromDataAnnotation: false /* Convention */);
			}
		}
	}
}