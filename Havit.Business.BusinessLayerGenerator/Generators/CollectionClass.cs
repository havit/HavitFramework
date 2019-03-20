using System;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class CollectionClass
	{
		public static void Generate(Table table, CsprojFile csprojFile)
		{
			string fileName = FileHelper.GetFilename(table, "Collection.cs", "");

			if (csprojFile != null)
			{
				csprojFile.Ensures(fileName);
			}

			if (File.Exists(FileHelper.ResolvePath(fileName)))
			{
				BusinessObjectUsings.RemoveObsoleteUsings(fileName);
			}
			else
			{
				CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName));

				BusinessObjectUsings.WriteUsings(writer);
				CollectionBaseClass.WriteNamespaceBegin(writer, table);
				WriteClassBegin(writer, table, false);
				CollectionBaseClass.WriteClassEnd(writer);
				CollectionBaseClass.WriteNamespaceEnd(writer);

				writer.Save();
			}
		}

		public static void WriteClassBegin(CodeWriter writer, Table table, bool partial)
		{
			writer.WriteCommentSummary(String.Format("Kolekce business objektů typu {0}.", ClassHelper.GetClassFullName(table)));

			if (partial)
			{
				//				writer.WriteLine("[Serializable]");
				writer.WriteLine(
					String.Format("{0} partial class {1}",
					TableHelper.GetAccessModifier(table),
					ClassHelper.GetCollectionClassName(table)));
			}
			else
			{
				writer.WriteLine(
					String.Format("{0} partial class {1} : {2}",
					TableHelper.GetAccessModifier(table),
					ClassHelper.GetCollectionClassName(table),
					ClassHelper.GetCollectionBaseClassName(table)));
			}

			writer.WriteLine("{");
			writer.WriteLine("");
		}
	}
}
