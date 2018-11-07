using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
    public static class FileBasedStoredProcedureGenerator
	{
		public static void Generate(List<DbStoredProcedure> storedProcedures, CsprojFile entityCsprojFile)
		{
		    foreach (DbStoredProcedure dbStoredProcedure in storedProcedures)
			{
				string fileName = Path.Combine(Path.GetDirectoryName(entityCsprojFile.Path), dbStoredProcedure.GeneratedFile);
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));

				string[] lines = dbStoredProcedure.StoredProcedure.Script(new ScriptingOptions())
					.Cast<string>()
					.SelectMany(l => l.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
					.SkipWhile(s => !s.Trim().StartsWith("CREATE"))
					.Where(s => s.Trim() != "GO")
					.ToArray();

				File.WriteAllLines(fileName, lines);

				entityCsprojFile.EnsuresEmbeddedResource(GetEntityProjectRelativePath(dbStoredProcedure.GeneratedFile));
			}
		}

		private static string GetEntityProjectRelativePath(string file) => file.Substring(file.IndexOf('\\') + 1);
	}
}