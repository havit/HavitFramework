using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.Views
{
	/// <summary>
	/// Bázová trieda pre definovanie DB Injections na views.
	/// </summary>
	public class ViewDbInjector : IDbInjector
    {
		/// <summary>
		/// Vytvorí <see cref="ViewDbInjection"/> objekt z create skriptu v resources.
		/// </summary>
		/// <param name="createScriptResourceName">Názov resource s create skriptom pre view.</param>
		/// <param name="resourceAssembly">Assembly, ve které se resource hledá. Volitelný parametr. Není-li uveden, hledá se v assembly dle GetDefaultResourceAssembly().</param>
		/// <returns><see cref="ViewDbInjection"/> objekt reprezentujúci view.</returns>
		protected ViewDbInjection Procedure(string createScriptResourceName, Assembly resourceAssembly = null)
		{
			resourceAssembly = resourceAssembly ?? GetDefaultResourceAssembly();

            if (!resourceAssembly.GetManifestResourceNames().Contains(createScriptResourceName))
            {
                throw new ArgumentException($"Invalid embedded resource name {createScriptResourceName}", nameof(createScriptResourceName));
            }

            using (var textStream = new StreamReader(resourceAssembly.GetManifestResourceStream(createScriptResourceName)))
            {
                string createScript = textStream.ReadToEnd();
                return new ViewDbInjection { CreateSql = createScript, ViewName = ParseViewName(createScript) };
            }
        }

		/// <summary>
		/// Assembly, ktorá obsahuje resources so skriptami. Štandardne je to assembly, v ktorej je definovaná trieda zdedená od <see cref="ViewDbInjection"/>.
		/// </summary>
		private Assembly GetDefaultResourceAssembly() => GetType().Assembly;

        private string ParseViewName(string createScript) => Regex.Match(createScript, @"CREATE(\s+)VIEW(\s+)(\[.*?\]\.)?\[?(?<proc_name>[\w]*)\]?").Groups["proc_name"].Value;
    }
}