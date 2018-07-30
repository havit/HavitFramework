using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Havit.Business.CodeMigrations.DbInjections
{
    public class StoredProcedureDbInjector<T>
    {
        protected StoredProcedureDbInjection Procedure(string createScriptResourceName)
        {
            if (!ResourceAssembly.GetManifestResourceNames().Contains(createScriptResourceName))
            {
                throw new ArgumentException($"Invalid embedded resource name {createScriptResourceName}", nameof(createScriptResourceName));
            }

            using (var textStream = new StreamReader(ResourceAssembly.GetManifestResourceStream(createScriptResourceName)))
            {
                string createScript = textStream.ReadToEnd();
                return new StoredProcedureDbInjection { CreateSql = createScript, ProcedureName = ParseProcedureName(createScript) };
            }
        }

        protected virtual Assembly ResourceAssembly => GetType().Assembly;

        private string ParseProcedureName(string createScript) => Regex.Match(createScript, @"CREATE(\s+)PROCEDURE(\s+)(\[.*?\]\.)?\[(?<proc_name>.*)\]").Groups["proc_name"].Value;
    }
}