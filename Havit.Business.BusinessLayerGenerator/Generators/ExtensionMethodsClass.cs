using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class ExtensionMethodsClass
	{
		#region Generate
		public static void Generate(List<Table> tables, CsprojFile csprojFile, SourceControlClient sourceControlClient)
		{
			tables.RemoveAll(table => TableHelper.IsJoinTable(table));

			// tabulky rozdělíme po namespaces
			// extension metody dáváme do příslušného namespace

			var tableNamespacesGroups = tables.ToLookup(table => NamespaceHelper.GetNamespaceName(table, false));

			foreach (var tableNamespacesGroup in tableNamespacesGroups.OrderBy(group => group.Key, StringComparer.InvariantCultureIgnoreCase))
			{
				string namespaceText = String.IsNullOrEmpty(tableNamespacesGroup.Key) ? "default namespace" : String.Format("namespace {0}", tableNamespacesGroup.Key);
				ConsoleHelper.WriteLineInfo("Extension methods ({0})", namespaceText);

				string filename = FileHelper.GetFilename(tableNamespacesGroup.Key, "ExtensionMethods", ".partial.cs", FileHelper.GeneratedFolder);

				if (csprojFile != null)
				{
					csprojFile.Ensures(filename);
				}

				CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(filename), sourceControlClient);

				BusinessObjectUsings.WriteUsings(writer);

				writer.WriteLine("namespace " + NamespaceHelper.GetNamespaceName(tableNamespacesGroup.ElementAt(0), true));
				writer.WriteLine("{");

				writer.WriteCommentSummary("Rozšiřující metody.");

				writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
				writer.WriteGeneratedCodeAttribute();

				writer.WriteLine("public static partial class ExtensionMethods");
				writer.WriteLine("{");

				foreach (Table table in tableNamespacesGroup.OrderBy(item => ClassHelper.GetClassFullName(item), StringComparer.InvariantCultureIgnoreCase))
				{
					WriteExtensionMethods(writer, table);
				}
				writer.WriteLine("}"); // class			

				writer.WriteLine("}"); // namespace
				
				writer.Save();
			}
		}
		#endregion

		#region WriteExtensionMethods
		private static void WriteExtensionMethods(CodeWriter writer, Table table)
		{
			writer.WriteOpenRegion(String.Format("IEnumerable<{0}>.ToCollection", ClassHelper.GetCollectionClassFullName(table)));
			writer.WriteCommentSummary(String.Format("Vytvoří {0} z IEnumerable<{1}>.", ClassHelper.GetCollectionClassFullName(table), ClassHelper.GetClassFullName(table)));
			writer.WriteLine(String.Format("{0} static {1} ToCollection(this IEnumerable<{2}> objects)",
				TableHelper.GetAccessModifier(table),
				ClassHelper.GetCollectionClassFullName(table),
				ClassHelper.GetClassFullName(table)));
			writer.WriteLine("{");
			writer.WriteLine(String.Format("return new {0}(objects);", ClassHelper.GetCollectionClassFullName(table)));
			writer.WriteLine("}"); // method
			writer.WriteCloseRegion();
		}
		#endregion
	}
}