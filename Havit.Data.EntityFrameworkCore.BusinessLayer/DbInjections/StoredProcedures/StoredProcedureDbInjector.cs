using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures
{
	/// <summary>
	/// Bázová trieda pre definovanie DB Injections na uložené procedúry.
	/// </summary>
	public class StoredProcedureDbInjector : IDbInjector
    {
		/// <summary>
		/// Vytvorí <see cref="StoredProcedureDbInjection"/> objekt z create skriptu v resources.
		/// </summary>
		/// <param name="createScriptResourceName">Názov resource s create skriptom pre uloženú procedúru.</param>
		/// <param name="resourceAssembly">Assembly, ve které se resource hledá. Volitelný parametr. Není-li uveden, hledá se v assembly dle GetDefaultResourceAssembly().</param>
		/// <returns><see cref="StoredProcedureDbInjection"/> objekt reprezentujúci uloženú procedúru.</returns>
		protected StoredProcedureDbInjection Procedure(string createScriptResourceName, Assembly resourceAssembly = null)
		{
			resourceAssembly = resourceAssembly ?? GetDefaultResourceAssembly();

            if (!resourceAssembly.GetManifestResourceNames().Contains(createScriptResourceName))
            {
                throw new ArgumentException($"Invalid embedded resource name {createScriptResourceName}", nameof(createScriptResourceName));
            }

            using (var textStream = new StreamReader(resourceAssembly.GetManifestResourceStream(createScriptResourceName)))
            {
                string createScript = textStream.ReadToEnd();
                return new StoredProcedureDbInjection { CreateSql = createScript, ProcedureName = ParseProcedureName(createScript) };
            }
        }

		/// <summary>
		/// Assembly, ktorá obsahuje resources so skriptami. Štandardne je to assembly, v ktorej je definovaná trieda zdedená od <see cref="StoredProcedureDbInjection"/>.
		/// </summary>
		private Assembly GetDefaultResourceAssembly() => GetType().Assembly;

        private string ParseProcedureName(string createScript) => Regex.Match(createScript, @"CREATE(\s+)PROCEDURE(\s+)(\[.*?\]\.)?\[?(?<proc_name>[\w]*)\]?").Groups["proc_name"].Value;
    }
}