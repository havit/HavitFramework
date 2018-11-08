using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Havit.Data.EntityFrameworkCore.Conventions;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions
{
	/// <summary>
	/// Konvencia pre nastavenie MS_Description extended property na entitách pomocou XML komentárov. Je nutné, aby assembly s modelom mala zapnuté generovanie XML komentárov. Súbor s XML komentárom by mal byť umiestnený vedľa assembly samotnej s príponou .XML
	/// 
	/// <remarks>U properties pre cudzie kľúče sa vezme XML komentár z navigačnej property (ak existuje a na property pre cudzí kľúč nebol už definovaný komentár).</remarks>
	/// </summary>
	public class XmlCommentsForDescriptionPropertyConvention : IModelConvention
	{
		private const string MsDescriptionExtendedProperty = "MS_Description";

		/// <inheritdoc />
		public void Apply(ModelBuilder modelBuilder)
		{
			var xmlCommentParser = new XmlCommentParser();

			var groupedByAssemblies = modelBuilder.Model
				.GetApplicationEntityTypes()
				.GroupBy(entityType => entityType.ClrType.Assembly);

			foreach (IGrouping<Assembly, IMutableEntityType> assemblyEntities in groupedByAssemblies)
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

		private static void CreateDescriptionExtendedProperties(XmlCommentFile xmlCommentFile, IEnumerable<IMutableEntityType> entities)
		{
			foreach (IMutableEntityType entityType in entities)
			{
				XmlCommentType xmlCommentType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == entityType.ClrType.FullName);
				if (xmlCommentType == null)
				{
					continue;
				}

				entityType.AddExtendedProperties(new Dictionary<string, string>
				{
					{ MsDescriptionExtendedProperty, EncodeValue(xmlCommentType.Summary) }
				});

				foreach (IMutableProperty property in entityType.GetProperties())
				{
					XmlCommentMember xmlCommentMember = xmlCommentType.Properties.FirstOrDefault(p => p.Name == (xmlCommentType.Name + "." + property.Name));
					if (xmlCommentMember == null)
					{
						var fk = entityType.FindForeignKeys(property).FirstOrDefault();
						if (fk?.DependentToPrincipal != null)
						{
							xmlCommentMember = xmlCommentType.Properties.FirstOrDefault(p => p.Name.EndsWith(fk.DependentToPrincipal.PropertyInfo.Name));
						}
					}

					if (xmlCommentMember != null)
					{
						property.AddExtendedProperties(new Dictionary<string, string>
						{
							{ MsDescriptionExtendedProperty, EncodeValue(xmlCommentMember.Summary) }
						});
					}
				}

				foreach (IMutableNavigation collection in entityType.GetNavigations().Where(n => n.IsCollection()))
				{
					if (collection.Name == "Localizations" && collection.ForeignKey.DeclaringEntityType.IsLocalizationEntity())
					{
						// Localizations property cannot have Collection_Description extended property defined
						continue;
					}

					XmlCommentMember xmlCommentCollection = xmlCommentType.Properties.FirstOrDefault(p => p.Name.EndsWith(collection.PropertyInfo.Name));

					if (xmlCommentCollection != null)
					{
						entityType.AddExtendedProperties(new Dictionary<string, string>
						{
							{ $"Collection_{collection.PropertyInfo.Name}_Description", EncodeValue(xmlCommentCollection.Summary) }
						});
					}
				}

			}
		}

		private static string EncodeValue(string value) => value.Trim().Replace("\n", "\\n");
	}
}