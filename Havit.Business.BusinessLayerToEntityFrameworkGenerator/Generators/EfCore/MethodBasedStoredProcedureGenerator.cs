using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers;
using Havit.Business.BusinessLayerToEntityFrameworkGenerator.Metadata;
using Microsoft.SqlServer.Management.Smo;
using NamespaceHelper = Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers.NamingConventions.NamespaceHelper;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators.EfCore
{
	public static class MethodBasedStoredProcedureGenerator
    {
        public static void Generate(List<DbStoredProcedure> storedProcedure, GeneratedModel model, CsprojFile modelCsprojFile, SourceControlClient sourceControlClient)
        {
            var groupedProcedures = storedProcedure
                .GroupBy(sp => sp.EntityName);

            foreach (IGrouping<string, DbStoredProcedure> fileGroup in groupedProcedures)
            {
                GeneratedModelClass entityClass = null;
                string relativeFilePath;
                string spClassName;
                if (fileGroup.Key == null)
                {
                    spClassName = "UnnamedStoredProcedures";
                }
                else
                {
                    spClassName = String.Format("{0}Procedures", fileGroup.Key);
                    entityClass = model.GetEntityByTable(fileGroup.Key);
                }
                relativeFilePath = BusinessLayerGenerator.Helpers.FileHelper.GetFilename("StoredProcedures", spClassName, ".cs", "");

                string fileName = Path.Combine(Path.GetDirectoryName(modelCsprojFile.Path), "Entity", relativeFilePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                CodeWriter codeWriter = new CodeWriter(fileName, sourceControlClient);

                WriteUsings(codeWriter, entityClass);
                WriteNamespaceClassConstructorBegin(codeWriter, entityClass, spClassName);

                foreach (DbStoredProcedure dbStoredProcedure in fileGroup)
                {
                    WriteMethodBegin(codeWriter, dbStoredProcedure);
                    WriteProcedureStatement(codeWriter, dbStoredProcedure);
                    WriteMethodEnd(codeWriter);
                }

                WriteNamespaceClassEnd(codeWriter);

                codeWriter.Save();
            }
        }

        /// <summary>
        /// Zapíše usings na všechny možné potřebné namespace.
        /// </summary>
        private static void WriteUsings(CodeWriter writer, GeneratedModelClass entityClass)
        {
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text;");
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes;");
			writer.WriteLine("using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures;");
			writer.WriteLine("using static Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes.DataLoadPowerType;");
			writer.WriteLine("using ResultType = Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes.StoredProcedureResultType;");
            if (entityClass != null)
            {
                writer.WriteLine($"using {entityClass.Namespace};");
            }

            writer.WriteLine();
        }

        private static void WriteNamespaceClassConstructorBegin(CodeWriter writer, GeneratedModelClass entityClass, string spClassName)
        {
            writer.WriteLine("namespace " + String.Format("{0}.StoredProcedures", NamespaceHelper.GetDefaultNamespace("Entity")));
            writer.WriteLine("{");

	        if (entityClass != null)
	        {
				writer.WriteLine(String.Format("[Attach(nameof({0}))]", entityClass.Name));
	        }
            writer.WriteLine(String.Format("public class {0} : StoredProcedureDbInjector", spClassName));
            writer.WriteLine("{");
        }

        private static void WriteMethodBegin(CodeWriter writer, DbStoredProcedure dbStoredProcedure)
        {
	        string result = dbStoredProcedure.StoredProcedure.GetStringExtendedProperty("Result");
	        if (!string.IsNullOrEmpty(result))
	        {
				writer.WriteLine(String.Format("[Result(ResultType.{0})]", result));
	        }

	        string dataLoadPower = dbStoredProcedure.StoredProcedure.GetStringExtendedProperty("DataLoadPower");
	        if (!string.IsNullOrEmpty(dataLoadPower))
			{
				writer.WriteLine(String.Format("[DataLoadPower({0})]", dataLoadPower));
	        }
            writer.WriteLine(String.Format("public StoredProcedureDbInjection {0}()", dbStoredProcedure.Name));
            writer.WriteLine("{");
        }

        private static void WriteProcedureStatement(CodeWriter writer, DbStoredProcedure dbStoredProcedure)
        {
	        string directoryName = Path.GetDirectoryName(dbStoredProcedure.GeneratedFile).Replace(Path.DirectorySeparatorChar, '.').Replace("Entity.", "");
	        var resourceName = String.Format("{0}.{1}.{2}", NamespaceHelper.GetDefaultNamespace("Entity"), directoryName, Path.GetFileName(dbStoredProcedure.GeneratedFile));
	        writer.WriteLine($"return Procedure(\"{resourceName}\");");
        }

        private static void WriteMethodEnd(CodeWriter writer)
        {
            writer.WriteLine("}");
        }

        private static void WriteNamespaceClassEnd(CodeWriter writer)
        {
            writer.WriteLine("}");
            writer.WriteLine("}");
        }
    }
}