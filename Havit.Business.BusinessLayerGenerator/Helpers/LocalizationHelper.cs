using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

/// <summary>
/// Pomocné třídy pro lokalizace.
/// </summary>
public static class LocalizationHelper
{
	/// <summary>
	/// Suffix názvu tabulky - identifikuje, že tabulka obsahuje lokalizační data.
	/// </summary>
	public static readonly string[] LocalizationTableNameSuffixes = new string[] { "Localization", "_Lang" };

	/// <summary>
	/// Název sloupce odkazující na tabulku jazyků v lokalizační tabulce.
	/// </summary>
	public const string LanguageForeignKeyColumnName = "LanguageID";

	/// <summary>
	/// Vrací true, pokud jde o tabulku nesoucí lokalizovaná data.
	/// </summary>
	public static bool IsLocalizationTable(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return false;
		}

		foreach (string localizationTableNameSuffix in LocalizationTableNameSuffixes)
		{
			if (table.Name.EndsWith(localizationTableNameSuffix) && (table.Name.Length > localizationTableNameSuffix.Length))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Vrací true, pokud jde o tabulku vlastnící jinou tabulku s lokalizovanými daty.
	/// </summary>
	public static bool IsLocalizedTable(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return false;
		}

		foreach (string localizationTableNameSuffix in LocalizationTableNameSuffixes)
		{
			if (DatabaseHelper.FindTable(table.Name + localizationTableNameSuffix, table.Schema) != null)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Vrátí tabulku, která je lokalizována tabulkou, která je předána jako parametr.
	/// </summary>
	public static Table GetLocalizationParentTable(Table localizationTable)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return null;
		}
		string localizationTableName = localizationTable.Name;

		foreach (string localizationTableNameSuffix in LocalizationTableNameSuffixes)
		{
			if (localizationTableName.EndsWith(localizationTableNameSuffix))
			{
				string parentName = localizationTableName.Substring(0, localizationTableName.Length - localizationTableNameSuffix.Length);
				Table result = DatabaseHelper.FindTable(parentName, localizationTable.Schema, includeIgnored: GeneratorSettings.Strategy == GeneratorStrategy.HavitCodeFirst);
				if (result != null)
				{
					return result;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Vrátí tabulku, která lokalizuje tabulku, která je předána jako parametr.
	/// </summary>
	public static Table GetLocalizationTable(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return null;
		}

		foreach (string localizationTableNameSuffix in LocalizationTableNameSuffixes)
		{
			Table result = DatabaseHelper.FindTable(table.Name + localizationTableNameSuffix, table.Schema);
			if (result != null)
			{
				return result;
			}
		}
		return null;
	}

	/// <summary>
	/// Vrátí sloupec odkazující na tabulku, která je lokalizována.
	/// </summary>
	public static Column GetParentLocalizationColumn(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return null;
		}

		if (LanguageHelper.IsLanguageTable(table))
		{
			return table.Columns["ParentLanguageID"];
		}
		return table.Columns[TableHelper.GetPrimaryKey(LocalizationHelper.GetLocalizationParentTable(table)).Name];
	}

	/// <summary>
	/// Vrátí sloupec odkazující do tabulky jazyků (dle konvence pojmenování, vrací tedy sloupec "LanguageID").
	/// </summary>
	public static Column GetLanguageColumn(Table table)
	{
		return table.Columns[LanguageForeignKeyColumnName];
	}
}
