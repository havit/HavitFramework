using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;

/// <summary>
/// Konvencia pre nastavenie MS_Description extended property na entitách pomocou XML komentárov. Je nutné, aby assembly s modelom mala zapnuté generovanie XML komentárov. Súbor s XML komentárom by mal byť umiestnený vedľa assembly samotnej s príponou .XML
/// 
/// <remarks>U properties pre cudzie kľúče sa vezme XML komentár z navigačnej property (ak existuje a na property pre cudzí kľúč nebol už definovaný komentár).</remarks>
/// </summary>
public class XmlCommentsForDescriptionPropertyConvention : IModelFinalizingConvention
{
	internal const string MsDescriptionExtendedProperty = "MS_Description";

	public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
	{
		// suppress neřešíme
		// systémové tabulky řešeny v GetApplicationEntityTypes()

		var xmlCommentParser = new XmlCommentParser();

		var groupedByAssemblies = modelBuilder.Metadata
			.GetApplicationEntityTypes()
			.Cast<IConventionEntityType>()
			.GroupBy(entityType => entityType.ClrType.Assembly);

		foreach (IGrouping<Assembly, IConventionEntityType> assemblyEntities in groupedByAssemblies)
		{
			string xmlFile = GetXmlCommentsFileFromAssembly(assemblyEntities.Key);
			if (string.IsNullOrEmpty(xmlFile))
			{
				continue;
			}

			var document = XDocument.Load(xmlFile);
			XmlCommentFile xmlCommentFile = xmlCommentParser.ParseFile(document);

			CreateDescriptionExtendedProperties(xmlCommentFile, assemblyEntities);
		}
	}

	private static string GetXmlCommentsFileFromAssembly(Assembly assembly)
	{
		var assemblyFile = new FileInfo(new Uri(assembly.Location).LocalPath);
		var xmlFile = $"{Path.GetFileNameWithoutExtension(assemblyFile.Name)}.xml";
		return assemblyFile.Directory?.GetFiles(xmlFile).FirstOrDefault()?.FullName;
	}

	private static void CreateDescriptionExtendedProperties(XmlCommentFile xmlCommentFile, IEnumerable<IConventionEntityType> entities)
	{
		foreach (IConventionEntityType entityType in entities)
		{
			string preprocessedTypeName = entityType.ClrType.FullName.Replace('+', '.');
			XmlCommentType xmlCommentType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == preprocessedTypeName);
			if (xmlCommentType == null)
			{
				continue;
			}

			// XmlCommentType can be "shadow type" - i.e. it has no tags on its own (and does not exist in source XML file)
			if (!string.IsNullOrWhiteSpace(xmlCommentType.Summary))
			{
				entityType.AddExtendedProperties(new Dictionary<string, string>
					{
						{ MsDescriptionExtendedProperty, EncodeValue(xmlCommentType.Summary) }
					},
					fromDataAnnotation: false /* Convention */);
			}

			foreach (IConventionProperty property in entityType.GetProperties())
			{
				XmlCommentMember xmlCommentMember = xmlCommentType.Properties.FirstOrDefault(p => p.Name == (xmlCommentType.Name + "." + property.Name));
				if (xmlCommentMember == null)
				{
					var fk = entityType.FindForeignKeys(property).FirstOrDefault();
					if (fk?.DependentToPrincipal != null)
					{
						xmlCommentMember = xmlCommentType.Properties.FirstOrDefault(p => p.Name == (xmlCommentType.Name + "." + fk.DependentToPrincipal.PropertyInfo.Name));
					}
				}

				if (xmlCommentMember != null)
				{
					property.AddExtendedProperties(new Dictionary<string, string>
						{
							{ MsDescriptionExtendedProperty, EncodeValue(xmlCommentMember.Summary) }
						}, fromDataAnnotation: false /* Convention */);
				}
			}

			foreach (INavigation collection in entityType.GetNavigations().Where(n => n.IsCollection))
			{
				if (collection.Name == "Localizations" && collection.ForeignKey.DeclaringEntityType.IsBusinessLayerLocalizationEntity())
				{
					// Localizations property cannot have Collection_Description extended property defined
					continue;
				}

				XmlCommentMember xmlCommentCollection = xmlCommentType.Properties.FirstOrDefault(p => p.Name == (xmlCommentType.Name + "." + collection.PropertyInfo.Name));

				if (xmlCommentCollection != null)
				{
					entityType.AddExtendedProperties(new Dictionary<string, string>
						{
							{ $"Collection_{collection.PropertyInfo.Name}_Description", EncodeValue(xmlCommentCollection.Summary) }
						},
						fromDataAnnotation: false);
				}
			}

		}
	}

	private static string EncodeValue(string value)
	{
		return string.Join("\r\n", value.Trim().Trim('\n', '\r').Split('\n').Select(item => item.Trim()));
	}
}