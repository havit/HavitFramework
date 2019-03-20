using System;
using System.IO;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class FileHelper
	{
		public const string GeneratedFolder = "_generated";

		/// <summary>
		/// Vrátí jméno generovaného souboru, namespaces tvoří adresářovou strukturu.
		/// </summary>
		public static string GetFilename(Table table, string suffix, string organizationFolder)
		{
			string namespaceName = NamespaceHelper.GetNamespaceName(table, false);
			string className = ClassHelper.GetClassName(table);
			return GetFilename(namespaceName, className, suffix, organizationFolder);
		}

		/// <summary>
		/// Vrátí jméno generovaného souboru, namespaces tvoří adresářovou strukturu.
		/// </summary>
		public static string GetFilename(string namespaceName, string className, string suffix, string organizationFolder)
		{
			string path = String.Format("{0}.{1}.{2}", namespaceName, organizationFolder, className);
			path = path.Replace("..", ".").Trim('.').Replace(".", "\\");

			return path + suffix;
		}

		/// <summary>
		/// Zkombinuje zadanou cestu s cestou v GeneratorSettings.
		/// </summary>
		internal static string ResolvePath(string path)
		{
			return Path.Combine(GeneratorSettings.OutputPath, path);
		}
	}
}