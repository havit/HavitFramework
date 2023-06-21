using System;
using System.Collections.ObjectModel;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

/// <summary>
/// Pomocné metody pro práci se SP.
/// </summary>
public static class StoredProcedureHelper
{
	/// <summary>
	/// Vrátí stored procedury připojené k tabulce pomocí Extended property Attach.
	/// </summary>
	public static Collection<StoredProcedure> GetStoredProcedures(Table table)
	{
		Collection<StoredProcedure> result = new Collection<StoredProcedure>();
		Database database = table.Parent;

		foreach (StoredProcedure procedure in database.StoredProcedures)
		{
			if (procedure.IsSystemObject)
			{
				continue;
			}

			if (StoredProcedureHelper.IsIgnored(procedure))
			{
				continue;
			}

			string attach = GetAttach(procedure);
			if (!String.IsNullOrEmpty(attach))
			{
				string[] parsedValues = attach.Split(new string[] { "." }, StringSplitOptions.None);
				Table attachTable = (parsedValues.Length == 1) ? DatabaseHelper.FindTable(parsedValues[0], procedure.Schema) : DatabaseHelper.FindTable(parsedValues[1], parsedValues[0]);
				if (attachTable == table)
				{
					result.Add(procedure);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Vrátí hodnotu extended property "Ignored" u stored procedury.
	/// </summary>
	public static bool IsIgnored(StoredProcedure procedure)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromStoredProcedure(procedure), "Ignored", procedure.Name) ?? false;
	}

	/// <summary>
	/// Vrátí hodnotu extended property "Attach" u stored procedury.
	/// </summary>
	public static string GetAttach(StoredProcedure procedure)
	{
		string result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "Attach");
		//if (String.IsNullOrEmpty(result) && procedure.Name.Contains("_"))
		//{
		//    result = procedure.Name.Substring(0, procedure.Name.IndexOf("_"));
		//}
		return result;
	}

	/// <summary>
	/// Vrátí DataLoadPower z Extended property u procedury. 
	/// Není-li hodnota uvedena, vrací se "Ghost".
	/// </summary>
	public static string GetDataLoadPower(StoredProcedure procedure)
	{
		string dataLoadPower = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "DataLoadPower");
		//if (String.IsNullOrEmpty(dataLoadPower))
		//{
		//    return "Partial";
		//}
		return dataLoadPower;
	}

	/// <summary>
	/// Vrátí DataLoadPower z Extended property "MethodName" u procedury. 
	/// Není-li hodnota uvedena, vrací se název SP.
	/// </summary>
	public static string GetMethodName(StoredProcedure procedure)
	{
		string methodName = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "MethodName");
		if (String.IsNullOrEmpty(methodName) && procedure.Name.Contains("_"))
		{
			methodName = procedure.Name.Substring(procedure.Name.IndexOf("_") + 1);
		}
		return methodName;
	}

	/// <summary>
	/// Vrátí hodnotu vlastnosti Result.
	/// </summary>
	public static string GetResult(StoredProcedure procedure)
	{
		string result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "Result");
		return result;
	}

	/// <summary>
	/// Vrátí informaci, co je výstupem SP.
	/// Není-li uvedeno, vrací None.
	/// </summary>
	public static StoreProcedureResultType GetResultType(StoredProcedure procedure)
	{
		string result = GetResult(procedure);
		if (String.IsNullOrEmpty(result))
		{
			return StoreProcedureResultType.None;
		}
		return (StoreProcedureResultType)Enum.Parse(typeof(StoreProcedureResultType), result, true);
	}

	/// <summary>
	/// Vrátí tabulku, která je návratovou hodnotou výsledku procedury.
	/// </summary>
	public static Table GetResultTypeTable(StoredProcedure procedure)
	{
		string tableName = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "ResultTypeTable");
		if (String.IsNullOrEmpty(tableName))
		{
			tableName = GetAttach(procedure);
		}

		string[] parsedValues = tableName.Split(new string[] { "." }, StringSplitOptions.None);
		if (parsedValues.Length > 2)
		{
			throw new ApplicationException(String.Format("Při zpracování SP '{0}' se nepodařilo zpracovat hodnotu '{1}'.", procedure.Name, tableName));
		}

		Table result = (parsedValues.Length == 1) ? DatabaseHelper.FindTable(parsedValues[0], procedure.Schema) : DatabaseHelper.FindTable(parsedValues[1], parsedValues[0]);
		if (result == null)
		{
			throw new ApplicationException(String.Format("Při zpracování SP '{0}' se nepodařilo nalézt tabulku '{1}'.", procedure.Name, tableName));
		}
		return result;
	}

	/// <summary>
	/// Vrátí přístupový modifikátor pro metodu wrapující Stored proceduru.
	/// </summary>
	public static string GetMethodAccessModifier(StoredProcedure procedure)
	{
		string result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromStoredProcedure(procedure), "MethodAccessModifier");
		if (String.IsNullOrEmpty(result))
		{
			result = "protected";
		}
		return result;
	}

	/// <summary>
	/// Vrátí příznak, zda se má nad ghosty vrácenými SP automaticky provést LoadAll.
	/// </summary>
	public static bool GetLoadGhosts(StoredProcedure procedure)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromStoredProcedure(procedure), "LoadGhosts", procedure.Name) ?? true;
	}
}
