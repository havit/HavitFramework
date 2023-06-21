using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

public static class LanguageHelper
{
	/// <summary>
	/// Název tabulky jazyků.
	/// </summary>
	public const string LanguageTableName = "Language";

	/// <summary>
	/// Schéma tabulky jazyků.
	/// </summary>
	public const string LanguageTableSchemaName = "dbo";

	/// <summary>
	/// Vrací true, pokud jde o tabulku jazyků.
	/// </summary>
	public static bool IsLanguageTable(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return false;
		}
		return table.Name == LanguageTableName;
	}

	/// <summary>
	/// Vrátí tabulku jazyků.
	/// </summary>
	public static Table GetLanguageTable()
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return null;
		}

		return DatabaseHelper.FindTable(LanguageTableName, LanguageTableSchemaName);
	}

	/// <summary>
	/// Vrátí sloupec pro UICulture.
	/// </summary>
	public static Column GetUICultureColumn()
	{
		Table table = GetLanguageTable();

		if (table.Columns.Contains("UICulture"))
		{
			return table.Columns["UICulture"];
		}
		return null;
	}
}
