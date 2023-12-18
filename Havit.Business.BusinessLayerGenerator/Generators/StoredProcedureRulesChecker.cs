using System;
using System.Collections.ObjectModel;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class StoredProcedureRulesChecker
{
	public static void CheckRules(Table table)
	{
		Collection<StoredProcedure> procedures = StoredProcedureHelper.GetStoredProcedures(table);

		foreach (StoredProcedure procedure in procedures)
		{
			CheckRules(table, procedure);
		}
	}

	private static void CheckRules(Table table, StoredProcedure procedure)
	{
		CheckDescription(table, procedure);
		CheckResultType(table, procedure);
		CheckMethodName(table, procedure);
		CheckDataLoadPower(table, procedure);
		CheckParameterType(table, procedure);
		CheckGetResourceValuesForLanguage(table, procedure);
	}

	private static void CheckDescription(Table table, StoredProcedure procedure)
	{
		string description = ExtendedPropertiesHelper.GetDescription(ExtendedPropertiesKey.FromStoredProcedure(procedure));
		if (String.IsNullOrEmpty(description))
		{
			ConsoleHelper.WriteLineWarning("SP {0}: Chybí popis (description).", procedure.Name);
		}
	}

	private static void CheckMethodName(Table table, StoredProcedure procedure)
	{
		string methodName = StoredProcedureHelper.GetMethodName(procedure);
		if (String.IsNullOrEmpty(methodName))
		{
			ConsoleHelper.WriteLineWarning("SP {0}: Chybí vlastnost MethodName.", procedure.Name);
		}
	}

	private static void CheckResultType(object talbe, StoredProcedure procedure)
	{
		string resultname = StoredProcedureHelper.GetResult(procedure);
		if (String.IsNullOrEmpty(resultname))
		{
			ConsoleHelper.WriteLineWarning("SP {0}: Chybí vlastnost Result.", procedure.Name);
			return;
		}
		if ((resultname != "None") && (resultname != "DataTable") && (resultname != "DataSet") && (resultname != "Object") && (resultname != "Collection"))
		{
			ConsoleHelper.WriteLineWarning("SP {0}: Neznámá hodnota vlastnosti Result.", procedure.Name);
		}
	}

	private static void CheckDataLoadPower(Table table, StoredProcedure procedure)
	{
		if ((StoredProcedureHelper.GetResultType(procedure) == StoreProcedureResultType.Object)
		 || (StoredProcedureHelper.GetResultType(procedure) == StoreProcedureResultType.Collection))
		{
			string dataLoadPower = StoredProcedureHelper.GetDataLoadPower(procedure);
			if (String.IsNullOrEmpty(dataLoadPower))
			{
				ConsoleHelper.WriteLineWarning("SP {0}: Chybí vlastnost DataLoadPower.", procedure.Name);
			}
			else if ((dataLoadPower != "Ghost") && (dataLoadPower != "PartialLoad") && (dataLoadPower != "FullLoad"))
			{
				ConsoleHelper.WriteLineWarning("SP {0}: Neznámá hodnota vlastnosti DataLoadPower.", procedure.Name);
			}
		}
	}

	private static void CheckParameterType(Table table, StoredProcedure procedure)
	{
		foreach (StoredProcedureParameter parameter in procedure.Parameters)
		{
			if ((parameter.DataType.SqlDataType == SqlDataType.UserDefinedType) && (parameter.DataType.Name == "IntArray"))
			{
				ConsoleHelper.WriteLineWarning("SP {0}, Parameter {1}: Datový typ IntArray již není podporován a nelze používat.", procedure.Name, parameter.Name);
			}
		}
	}

	private static void CheckGetResourceValuesForLanguage(Table table, StoredProcedure procedure)
	{
		if (procedure.Name == "ResourceClass_GetResourceValuesForLanguage")
		{
			ConsoleHelper.WriteLineWarning("SP {0}: Procedure nahrazena wrapperem resourců měla by být odstraněna (včetně třídy ResourceHelper a dalších závislostí).", procedure.Name);
		}
	}
}
