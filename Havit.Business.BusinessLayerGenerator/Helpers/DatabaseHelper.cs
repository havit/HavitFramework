using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class DatabaseHelper
	{
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

		/// <summary>
		/// Vrací seznam tabulek, ke které mohou být zpracovávány.
		/// Vrací se tabulky, které
		///  - nejsou systémové
		///  - nejsou ignorované
		/// </summary>
		public static List<Table> GetWorkingTables()
		{
			if (_workingTables == null)
			{
				List<Table> tables = new List<Table>();
				foreach (Table table in Database.Tables)
				{
					if (!table.IsSystemObject && !TableHelper.IsIgnored(table))
					{
						tables.Add(table);
					}
				}
				_workingTables = tables.OrderBy(item => item.Name, StringComparer.InvariantCultureIgnoreCase).ToList();
			}
			return _workingTables;
		}
		private static List<Table> _workingTables;

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

		/// <summary>
		/// Vrací výchozí hodnotu string trimmingu pro databázi.
		/// </summary>
		public static bool IsStringTrimming()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "StringTrimming", "Databáze") ?? false;
		}

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

		/// <summary>
		/// Přečte hodnotu GetAll_IncludeLocalizations v extended property databáze. Není-li uvedena, vrací true.
		/// </summary>
		public static bool GetGetAllIncludeLocalizations()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "GetAll_IncludeLocalizations", "Databáze") ?? true;
		}

		/// <summary>
		/// Přečte hodnotu LoadAll_IncludeLocalizations v extended property databáze. Není-li uvedena, vrací true.
		/// </summary>
		public static bool GetLoadAllIncludeLocalizations()
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "LoadAll_IncludeLocalizations", "Databáze") ?? true;
		}

		/// <summary>
        /// Vrací true, pokud databáze podporuje schéma ("dbo", apod.).
        /// </summary>
        public static bool IsDatabaseSchemaSupported()
        {
			return true;
        }

		/// <summary>
        /// Vratí výchozí hodnotu Ignore pro tabulky nastavenou na databázi v atributu "DefaultIgnoreOnTables".
        /// Není-li uvedeno, vrací false (tabulky nejsou ignorovány).
        /// </summary>
        public static bool GetDefaultIgnoredOnTables()
        {
            return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromDatabase(), "DefaultIgnoredOnTables", "Databáze") ?? false;
        }
	}
}
