using System.Collections.Generic;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
    public static class FileBasedStoredProcedureGenerator
	{
		public static void Generate(List<DbStoredProcedure> storedProcedures, CsprojFile modelCsprojFile)
		{
		    foreach (DbStoredProcedure dbStoredProcedure in storedProcedures)
            {
                string fileName = Path.Combine(Path.GetDirectoryName(modelCsprojFile.Path), "Entity", BusinessLayerGenerator.Helpers.FileHelper.GetFilename("Sql.StoredProcedures", dbStoredProcedure.Name, ".v01.sql", ""));
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                dbStoredProcedure.StoredProcedure.Script(new ScriptingOptions() { FileName = fileName });
            }
		}
	}
}