using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class MoneyPartialClass
	{
		#region Generate
		public static void Generate(Table currencyTable, CsprojFile csprojFile)
		{
			string fileName = FileHelper.GetFilename(NamespaceHelper.GetNamespaceName(currencyTable, false), "Money", ".partial.cs", FileHelper.GeneratedFolder);

			if (csprojFile != null)
			{
				csprojFile.Ensures(fileName);
			}

			CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName));

			BusinessObjectUsings.WriteUsings(writer);

			writer.WriteLine("namespace " + NamespaceHelper.GetNamespaceName(currencyTable));
			writer.WriteLine("{");

			writer.WriteCommentSummary("Třída reprezentující peněžní částky s měnou.");
			writer.WriteLine("public partial class Money : MoneyBase");
			writer.WriteLine("{");
			MoneyBaseClass.WriteConstructors(writer, currencyTable, "Money", false);
			writer.WriteLine("}");

			writer.WriteLine("}");
			
			writer.Save();
		}
		#endregion
	}
}
