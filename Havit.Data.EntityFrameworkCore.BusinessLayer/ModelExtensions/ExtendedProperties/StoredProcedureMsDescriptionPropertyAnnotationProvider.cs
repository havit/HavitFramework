using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties
{
	public class StoredProcedureMsDescriptionPropertyAnnotationProvider : ModelExtensionAnnotationProvider<StoredProcedureModelExtension>
	{
		private readonly XmlCommentFileCache xmlCommentFileCache = new XmlCommentFileCache();

		protected override List<IAnnotation> GetAnnotations(StoredProcedureModelExtension dbAnnotation, MemberInfo memberInfo)
		{
			if (memberInfo is MethodInfo methodInfo)
			{
				string summary = GetSummaryXmlCommentTagForMethod(methodInfo);
				if (summary != null)
				{
					return ExtendedPropertiesForExtraDatabaseObjectsBuilder.ForProcedure(new Dictionary<string, string>()
					{
						{ "MS_Description", summary }
					}, dbAnnotation.ProcedureName).ToList();
				}
			}

			return new List<IAnnotation>();
		}

		protected override List<StoredProcedureModelExtension> GetModelExtensions(List<IAnnotation> annotations)
		{
			return new List<StoredProcedureModelExtension>();
		}

		private string GetSummaryXmlCommentTagForMethod(MethodInfo methodInfo)
		{
			var xmlFile = xmlCommentFileCache.GetOrLoadXmlCommentFile(methodInfo.DeclaringType.Assembly);
			if (xmlFile == null)
			{
				return null;
			}

			return xmlFile.FindMethod(methodInfo)?.Summary;
		}


		private class XmlCommentFileCache
		{
			private readonly Dictionary<string, XmlCommentFile> files = new Dictionary<string, XmlCommentFile>();
			private readonly XmlCommentParser xmlCommentParser = new XmlCommentParser();

			public XmlCommentFile GetOrLoadXmlCommentFile(Assembly assembly)
			{
				string xmlFile = GetXmlCommentsFileFromAssembly(assembly);
				if (string.IsNullOrEmpty(xmlFile))
				{
					return null;
				}

				if (files.TryGetValue(xmlFile, out var file))
				{
					return file;
				}

				var document = XDocument.Load(xmlFile);

				file = xmlCommentParser.ParseFile(document);
				files[xmlFile] = file;

				return file;
			}

			private static string GetXmlCommentsFileFromAssembly(Assembly assembly)
			{
				var assemblyFile = new FileInfo(new Uri(assembly.Location).LocalPath);
				var xmlFile = $"{Path.GetFileNameWithoutExtension(assemblyFile.Name)}.xml";
				return assemblyFile.Directory?.GetFiles(xmlFile).FirstOrDefault()?.FullName;
			}
		}

	}
}