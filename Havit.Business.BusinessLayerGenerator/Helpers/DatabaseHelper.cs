using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class DatabaseHelper
	{
		#region Property Database (static)
		/// <summary>
		/// Vrátí aktuální databázi.
		/// </summary>
		public static Database Database
		{
			get
			{
				if (_database == null)
				{
					throw new ApplicationException("Databáze nebyla nastavena.");
				}
				return _database;
			}
			set
			{
				_database = value;
			}
		}
		private static Database _database;
		#endregion

		#region GetWorkingTables
		/// <summary>
		/// Vrací seznam tabulek, ke které mohou být zpracovávány.
		/// Vrací se tabulky, které
		///  - nejsou systémové
		///  - nejsou ignorované
		/// </summary>
		public static List<Table> GetWorkingTables()
		{
			List<Table> result = new List<Table>();
			foreach (Table table in Database.Tables)
			{
				if (!table.IsSystemObject && !TableHelper.IsIgnored(table))
				{
					result.Add(table);
				}
			}
			return result.OrderBy(item => item.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
		}
		#endregion

		#region FindTable(string tableName, string schemaName)
		/// <summary>
		/// Vrátí tabulku na základě jejího názvu a názvu schématu.
		/// Vrátí null, pokud není žádná tabulka nalezena nebo pokuď je nalezená tabulka ignorovaná.
		/// </summary>
		public static Table FindTable(string tableName, string schemaName, bool includeIgnored = false)
		{
			Table result = Database.Tables[tableName, schemaName];

			if (includeIgnored || ((result != null) && !TableHelper.IsIgnored(result)))
			{
				return result;
			}

			return null;
		}
		#endregion

		#region IsStringTrimming
		/// <summary>
		/// Vrací výchozí hodnotu string trimmingu pro databázi.
		/// </summary>
		public static bool IsStringTrimming()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "StringTrimming", "Databáze") ?? false;
		}
		#endregion

		#region GetDbConnector
		/// <summary>
		/// Vrací použitý DbConnector dle vlastnosti DbConnector na databázi. Výchozí hodnotou je "DbConnector.Default".
		/// </summary>
		public static string GetDbConnector()
		{
			string dbconnector = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "DbConnector");
			if (String.IsNullOrEmpty(dbconnector))
			{
				return "DbConnector.Default";
			}
			else
			{
				return dbconnector;
			}
		}
		#endregion

		#region GetGetAllIncludeLocalizations
		/// <summary>
		/// Přečte hodnotu GetAll_IncludeLocalizations v extended property databáze. Není-li uvedena, vrací true.
		/// </summary>
		public static bool GetGetAllIncludeLocalizations()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "GetAll_IncludeLocalizations", "Databáze") ?? true;
		}
		#endregion

		#region GetGetAllIncludeLocalizations
		/// <summary>
		/// Přečte hodnotu LoadAll_IncludeLocalizations v extended property databáze. Není-li uvedena, vrací true.
		/// </summary>
		public static bool GetLoadAllIncludeLocalizations()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "LoadAll_IncludeLocalizations", "Databáze") ?? true;
		}
		#endregion
		
		#region IsSchemaSupported
        /// <summary>
        /// Vrací true, pokud databáze podporuje schéma ("dbo", apod.).
        /// </summary>
        public static bool IsDatabaseSchemaSupported()
        {
            return GeneratorSettings.TargetPlatform != TargetPlatform.SqlServerCe35;
        }
    	#endregion

        #region GetDefaultIgnoredOnTables
        /// <summary>
        /// Vratí výchozí hodnotu Ignore pro tabulky nastavenou na databázi v atributu "DefaultIgnoreOnTables".
        /// Není-li uvedeno, vrací false (tabulky nejsou ignorovány).
        /// </summary>
        public static bool GetDefaultIgnoredOnTables()
        {
            return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "DefaultIgnoredOnTables", "Databáze") ?? false;
        } 
        #endregion
    }
}
