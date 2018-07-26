using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.TfsClient;
using Havit.Business.BusinessLayerGenerator.Writers;
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
                WriteNamespaceClassConstructorBegin(codeWriter, spClassName);

                foreach (DbStoredProcedure dbStoredProcedure in fileGroup)
                {
                    WriteMethodBegin(codeWriter, dbStoredProcedure);
                    WriteCreateOrAlterStatement(codeWriter, dbStoredProcedure);
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
            if (entityClass != null)
            {
                writer.WriteLine($"using {entityClass.Namespace};");
            }

            writer.WriteLine();
        }

        private static void WriteNamespaceClassConstructorBegin(CodeWriter writer, string spClassName)
        {
            writer.WriteLine("namespace " + String.Format("{0}.StoredProcedures", NamespaceHelper.GetDefaultNamespace("Entity")));
            writer.WriteLine("{");

            writer.WriteLine(String.Format("public static class {0}", spClassName));
            writer.WriteLine("{");
        }

        private static void WriteMethodBegin(CodeWriter writer, DbStoredProcedure dbStoredProcedure)
        {
            var parameterList = "";
            if (!string.IsNullOrEmpty(dbStoredProcedure.EntityName))
            {
                parameterList = $"(this {dbStoredProcedure.EntityName} {ConventionsHelper.GetCammelCase(dbStoredProcedure.EntityName)})";
            }

            writer.WriteLine(String.Format("public static string {0}{1}", dbStoredProcedure.Name, parameterList));
            writer.WriteLine("{");
        }

        private static void WriteCreateOrAlterStatement(CodeWriter writer, DbStoredProcedure dbStoredProcedure)
        {
            writer.WriteLine("return @\"");

            var scriptingOptions = new ScriptingOptions();
            //typeof(ScriptingOptions).GetProperty("ScriptForAlter", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(scriptingOptions, true);

            string[] lines = dbStoredProcedure.StoredProcedure.Script(scriptingOptions)
                .Cast<string>()
                .SkipWhile(s => !s.Trim().StartsWith("CREATE"))
                .ToArray();

            writer.WriteLines(lines);

            writer.WriteLine("\";");
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