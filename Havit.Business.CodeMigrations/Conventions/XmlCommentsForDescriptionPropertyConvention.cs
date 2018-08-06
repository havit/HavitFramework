using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Havit.Business.CodeMigrations.ExtendedProperties;
using Havit.Business.CodeMigrations.XmlComments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Business.CodeMigrations.Conventions
{
	public static class XmlCommentsForDescriptionPropertyConvention
	{
		private const string MsDescriptionExtendedProperty = "MS_Description";

		public static void UseXmlCommentsForDescriptionProperty(this ModelBuilder modelBuilder)
		{
			var xmlCommentParser = new XmlCommentParser();

			var groupedByAssemblies = modelBuilder.Model.GetEntityTypes().GroupBy(entityType => entityType.ClrType.Assembly);

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
					XmlCommentMember xmlCommentMember = xmlCommentType.Properties.FirstOrDefault(p => p.Name.EndsWith(property.Name));
					if (xmlCommentMember != null)
					{
						property.AddExtendedProperties(new Dictionary<string, string>
						{
							{ MsDescriptionExtendedProperty, EncodeValue(xmlCommentMember.Summary) }
						});
					}
				}
			}
		}

		private static string EncodeValue(string value) => value.Trim().Replace("\n", "\\n");
	}
}