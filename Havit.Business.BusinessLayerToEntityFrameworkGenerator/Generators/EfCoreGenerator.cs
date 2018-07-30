using System;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata.MetadataSource;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
	public static class EfCoreGenerator
	{
		public static void Generate(Database database, CsprojFile modelCsprojFile, CsprojFile entityCsprojFile, SourceControlClient sourceControlClient)
		{
			// nalezneme tabulky, na jejichž základě se budou generovat třídy
			Console.BufferHeight = Int16.MaxValue - 1;

			var modelClassSource = new ModelClassSource();
			var modelClasses = modelClassSource.GetModelClasses(DatabaseHelper.Database);

		    var dbStoredProcedureSource = new DbStoredProcedureSource();

		    var model = new GeneratedModel(modelClasses);
		    var dbStoredProcedures = dbStoredProcedureSource.GetStoredProcedures(database, model);

		    ConsoleHelper.WriteLineInfo("Generuji model");
			foreach (GeneratedModelClass modelClass in modelClasses)
			{
				ConsoleHelper.WriteLineInfo(modelClass.Name);
				GeneratedModelClass generatedModelClass = EfCore.ModelClass.Generate(modelClass, modelCsprojFile, sourceControlClient);
				EfCore.EntityTypeConfigurationClass.Generate(model, generatedModelClass, entityCsprojFile, sourceControlClient);
			}

			ConsoleHelper.WriteLineInfo("Generuji uložené procedury");
			FileBasedStoredProcedureGenerator.Generate(dbStoredProcedures, entityCsprojFile);
			MethodBasedStoredProcedureGenerator.Generate(dbStoredProcedures, model, entityCsprojFile, sourceControlClient);
		}
	}
}