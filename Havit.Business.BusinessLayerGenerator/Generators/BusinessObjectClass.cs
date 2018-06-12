using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectClass
	{
		#region Generate
		public static void Generate(Table table, CsprojFile csprojFile, SourceControlClient sourceControlClient)
		{
			string fileName = FileHelper.GetFilename(table, ".cs", "");

			if (csprojFile != null)
			{
				csprojFile.Ensures(fileName);
			}

			if (File.Exists(FileHelper.ResolvePath(fileName)))
			{
				BusinessObjectUsings.RemoveObsoleteUsings(fileName, sourceControlClient);
			}
			else
			{
				CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName), sourceControlClient);

				BusinessObjectUsings.WriteUsings(writer);
				BusinessObjectPartialClass.WriteNamespaceClassBegin(writer, table, false);
				writer.WriteLine();
				BusinessObjectPartialClass.WriteNamespaceClassEnd(writer);

				writer.Save();
			}
		}
		#endregion	
	}
}