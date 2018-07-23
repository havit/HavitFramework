using System;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class StoredProcedureGenerator
	{
		public static void Generate(StoredProcedure storedProcedure, CsprojFile modelCsprojFile)
		{
			string fileName = Path.Combine(Path.GetDirectoryName(modelCsprojFile.Path), "Entity", BusinessLayerGenerator.Helpers.FileHelper.GetFilename("Sql.StoredProcedures", storedProcedure.Name, ".v01.sql", ""));
			Directory.CreateDirectory(Path.GetDirectoryName(fileName));
			storedProcedure.Script(new ScriptingOptions() { FileName = fileName });
		}
	}
}