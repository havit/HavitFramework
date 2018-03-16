using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.Entity.Conventions
{
	///// <summary>
	///// Pokud je třída lokalizována, pak její lokalizace obsahuje vlastosti Parent a Language. K cizím klíčům těchto vlastností je automaticky založen unikátní index.
	///// </summary>
 //   public class LocalizationTableIndexConvention : IPropertyAddedConvention
 //   {
	//	public InternalEntityTypeBuilder Apply(InternalEntityTypeBuilder entityTypeBuilder)
	//	{
	//		// TODO JK: Suppress

	//		bool isLocalizationType = entityTypeBuilder.Metadata.ClrType.GetInterfaces().Any(item => item.IsGenericType && (item.GetGenericTypeDefinition() == typeof(ILocalization<,>))); // zjistíme, zda entityTypeCSpace implementuje typeof(ILocalization<,>
	//		if (isLocalizationType)
	//		{
	//			IMutableNavigation parentProperty = entityTypeBuilder.Metadata.FindNavigation("Parent");
	//			IMutableNavigation languageProperty = entityTypeBuilder.Metadata.FindNavigation("Language");

	//			// pokud máme k dispozici vlastnosti (sloupce) LanguageId a ParentId (teoreticky mohou být v předkovi nebo nemusí vůbec existovat, protože interface ILocalization<,> je nepředepisuje, apod.)
	//			if ((parentProperty != null) && (languageProperty != null))
	//			{
	//				Console.WriteLine(String.Join("|", parentProperty.ForeignKey.Properties.Select(property => property.Name)));

	//				try
	//				{
	//					Console.WriteLine(String.Join("|", parentProperty.ForeignKey.Properties.Where(p => !p.IsShadowProperty).Select(property => property.Name)));
	//				}
	//				catch (Exception e)
	//				{
	//					Console.WriteLine(e);
	//				}

	//				//new List<string>{ "ParentId", "LanguageId" }.ToArray(),

	//				string[] indexProperties = parentProperty.ForeignKey.Properties.Where(property => !property.IsShadowProperty).Select(property => property.Name).Concat(languageProperty.ForeignKey.Properties.Where(property => !property.IsShadowProperty).Select(property => property.Name)).ToArray();
	//				if (indexProperties.Any())
	//				{
	//					entityTypeBuilder.HasIndex(
	//							indexProperties,
	//							ConfigurationSource.Convention)
	//						.IsUnique(true, ConfigurationSource.Convention);
	//				}
	//			}
	//		}

	//		return entityTypeBuilder;
	//	}

	//}
}
