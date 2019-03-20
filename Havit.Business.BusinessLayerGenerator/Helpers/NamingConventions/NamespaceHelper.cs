using System;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions
{
	public static class NamespaceHelper
	{
		/// <summary>
		/// Vrací namespace pro třídu dle dané tabulky. 
		/// Pokud je parametr withDefaultNamespace true, připojí na začátek názvu namespace 
		/// defaultní namespace z nastavení v GeneratorSettings.Namespace.
		/// 
		/// Pokud jde o tabulku obsahující lokalizovaná data, vrátí namespace tabulky, jejíž data jsou lokalizována.
		/// </summary>
		public static string GetNamespaceName(Table table, bool withDefaultNamespace = true)
		{
			if (LocalizationHelper.IsLocalizationTable(table))
			{
				return GetNamespaceName(LocalizationHelper.GetLocalizationParentTable(table), withDefaultNamespace);
			}

			string tableNamespace = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "Namespace");

			if (!withDefaultNamespace)
			{
				return tableNamespace ?? string.Empty;
			}

			if (String.IsNullOrEmpty(tableNamespace))
			{
				return GetDefaultNamespace();
			}
			else
			{
				return String.Format("{0}.{1}", GetDefaultNamespace(), tableNamespace);
			}
		}

		public static string GetDefaultNamespace()
		{
			string databaseNamespace = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "Namespace");
			if (!String.IsNullOrEmpty(databaseNamespace))
			{
				return databaseNamespace;
			}
			return GeneratorSettings.Namespace;
		}
	}
}
