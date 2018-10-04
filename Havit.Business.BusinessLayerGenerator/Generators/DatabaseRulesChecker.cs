using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class DatabaseRulesChecker
	{
		#region CheckRules
		/// <summary>
		/// Ověří pravidla nad tabulkou.
		/// </summary>
		public static void CheckRules(Database database)
		{
			CheckNamespace(database);
			CheckDefinedTypes(database);
		}
		#endregion

		#region CheckNamespace
		/// <summary>
		/// Ověří existenci pravidla pro namespace.
		/// </summary>
		private static void CheckNamespace(Database database)
		{
			if (String.IsNullOrEmpty(ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "Namespace")))
			{
				ConsoleHelper.WriteLineWarning("Databáze: Není zadán namespace. ");
				if (!String.IsNullOrEmpty(GeneratorSettings.Namespace))
				{
					ConsoleHelper.WriteLineWarning("Použije se namespace z příkazové řádky.");
				}
			}
			else
			{
				if (!String.IsNullOrEmpty(GeneratorSettings.Namespace))
				{
					ConsoleHelper.WriteLineWarning("Databáze obsahuje namespace, namespace z příkazové řádky bude ignorován.");
				}
			}
		}
		#endregion

		#region CheckDefinedTypes
		private static void CheckDefinedTypes(Database database)
		{
			if (!CheckDefinedClrIntArray(database) || !CheckDefinedIntArrayTableType(database))
			{
				// nezapisujeme jako samostatný warning, protože jde jen o dodatečnou informaci
				// zápis warningu by zvýšil počet warningů v summary na konci
				ConsoleHelper.WriteLineWithColor(Console.Error, ConsoleHelper.WarningColor, @"SQL Skripty na přechod z SQL 2005 na SQL 2008 a další jsou k dispozici ve složce \\topol.havit.local\Library\SQL.");
			}
		}
		#endregion

		#region CheckDefinedClrIntArray
		private static bool CheckDefinedClrIntArray(Database database)
		{
			UserDefinedType dataType = database.UserDefinedTypes["IntArray"];
			//if ((dataType == null) && (GeneratorSettings.TargetPlatform == TargetPlatform.SqlServer2005))
			//{
			//	ConsoleHelper.WriteLineWarning("Nebyl nalezen CLR datový typ IntArray a target platform je SQL Server 2005.");
			//	return false;
			//}

			if ((dataType != null) && (GeneratorSettings.TargetPlatform >= TargetPlatform.SqlServer2008))
			{
				ConsoleHelper.WriteLineWarning("Byl nalezen CLR datový typ IntArray, ale pro target platform s SQL Server 2008 (a novější) jej nepoužíváme.");
				return false;
			}

			return true;
		}
		#endregion

		#region CheckDefinedIntArrayTableType
		private static bool CheckDefinedIntArrayTableType(Database database)
		{
			UserDefinedTableType dataType = database.UserDefinedTableTypes["IntTable"];
			if ((dataType == null) && (GeneratorSettings.TargetPlatform >= TargetPlatform.SqlServer2008))
			{
				ConsoleHelper.WriteLineWarning("Nebyl nalezen datový typ IntTable, který je pro target platform SQL Server 2008 (a novější) vyžadován.");
				return false;
			}

			return true;
		}
		#endregion

	}
}
