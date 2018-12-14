using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class PropertiesClassObsolete
	{
		#region RemoveObsolete
		public static void RemoveObsolete(Table table, CsprojFile csprojFile)
		{
			string fileName = FileHelper.GetFilename(table, "Properties.cs", "_properties");

			if (csprojFile != null)
			{
				csprojFile.Remove(fileName);
			}

			// pokud soubor neexistuje, již jej netvoříme
			if (!File.Exists(FileHelper.ResolvePath(fileName)))
			{
				return;
			}

			//CodeWriter writer = new CodeWriter(fileName, sourceControlClient);

			//BusinessObjectUsings.WriteUsings(writer);

			//writer.WriteLine(String.Format("namespace {0}", NamespaceHelper.GetNamespaceName(table)));
			//writer.WriteLine("{");

			//writer.WriteCommentSummary(String.Format("Objektová reprezentace metadat vlastností typu {0}.", ClassHelper.GetClassName(table)));
			//writer.WriteLine(String.Format("{0} class {1} : {2}",
			//	TableHelper.GetAccessModifier(table),
			//	ClassHelper.GetPropertiesClassName(table),
			//	ClassHelper.GetPropertiesBaseClassName(table)));
			//writer.WriteLine("{");
			//writer.WriteLine();
			//writer.WriteLine("}");

			//writer.WriteLine("}");

			//if (writer.AlreadyExistsTheSame())
			//{
				string targetFilename = FileHelper.ResolvePath(fileName);
				File.Delete(targetFilename);					

				// odmažeme ještě prázdnou složku
				string folder = Path.GetDirectoryName(targetFilename);
				if (Directory.GetFileSystemEntries(folder).Length == 0)
				{
					Directory.Delete(folder);						
				}
			//}
			//else
			//{
			//	ConsoleHelper.WriteLineWarning("Soubor {0} se již nepoužívá, neobsahuje ale standardní obsah. Je potřeba jej zkontrolovat a smazat ručně.", fileName);
			//}
			
		}
		#endregion
	}
}
