using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class MoneyClass
{
	public static void Generate(Table currencyTable, CsprojFile csprojFile)
	{
		string fileName = FileHelper.GetFilename(NamespaceHelper.GetNamespaceName(currencyTable, false), "Money", ".cs", "");

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

			writer.WriteLine("namespace " + NamespaceHelper.GetNamespaceName(currencyTable));
			writer.WriteLine("{");

			writer.WriteLine("public partial class Money : MoneyBase");
			writer.WriteLine("{");
			writer.WriteLine();
			writer.WriteLine("}");

			writer.WriteLine("}");

			writer.Save();
		}
	}
}
