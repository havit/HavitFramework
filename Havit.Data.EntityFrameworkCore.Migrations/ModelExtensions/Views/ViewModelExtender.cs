using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views
{
	/// <summary>
	/// Bázová trieda pre definovanie DB Injections na views.
	/// </summary>
	public class ViewModelExtender : IModelExtender
    {
		/// <summary>
		/// Vytvorí <see cref="ViewModelExtension"/> objekt z create skriptu v resources.
		/// </summary>
		/// <param name="createScriptResourceName">Názov resource s create skriptom pre view.</param>
		/// <param name="resourceAssembly">Assembly, ve které se resource hledá. Volitelný parametr. Není-li uveden, hledá se v assembly dle GetDefaultResourceAssembly().</param>
		/// <returns><see cref="ViewModelExtension"/> objekt reprezentujúci view.</returns>
		protected ViewModelExtension View(string createScriptResourceName, Assembly resourceAssembly = null)
		{
			resourceAssembly = resourceAssembly ?? GetDefaultResourceAssembly();

            if (!resourceAssembly.GetManifestResourceNames().Contains(createScriptResourceName))
            {
                throw new ArgumentException($"Invalid embedded resource name {createScriptResourceName}", nameof(createScriptResourceName));
            }

            using (var textStream = new StreamReader(resourceAssembly.GetManifestResourceStream(createScriptResourceName)))
            {
                string createScript = textStream.ReadToEnd();
                return new ViewModelExtension { CreateSql = createScript, ViewName = ParseViewName(createScript) };
            }
        }

		/// <summary>
		/// Assembly, ktorá obsahuje resources so skriptami. Štandardne je to assembly, v ktorej je definovaná trieda zdedená od <see cref="ViewModelExtension"/>.
		/// </summary>
		private Assembly GetDefaultResourceAssembly() => GetType().Assembly;

        private string ParseViewName(string createScript) => Regex.Match(createScript, @"CREATE(\s+)VIEW(\s+)(\[.*?\]\.)?\[?(?<proc_name>[\w]*)\]?").Groups["proc_name"].Value;
    }
}