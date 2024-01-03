using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

public static class ExtendedPropertiesHelper
{
	/// <summary>
	/// Hledá hodnotu extended property dle klíče.
	/// </summary>
	/// <param name="extendedProperties">Extended Properties, kde se extended property hledá.</param>
	/// <param name="key">Klíč extended property.</param>
	/// <returns>
	///		Je-li extended property nalezena a je-li hodnota typu string, pak vrací hodnotu extended property.
	///		Jinak vrací null.
	/// </returns>
	public static string GetString(ExtendedPropertiesKey extendedProperties, string key)
	{
		key = key.Replace("_", ".");

		if (GetAllExtendedProperties().TryGetValue(extendedProperties, out Dictionary<string, string> items))
		{
			if (items.TryGetValue(key, out string value))
			{
				return value;
			}
		}
		return null;
	}

	/// <summary>
	/// Vrátí všechny hodnoty extended properties tabulky.
	/// </summary>
	public static List<KeyValuePair<string, string>> GetTableExtendedProperties(Table table)
	{
		if (GetAllExtendedProperties().TryGetValue(ExtendedPropertiesKey.FromTable(table), out Dictionary<string, string> result))
		{
			return result.ToList();
		}
		return new List<KeyValuePair<string, string>>();
	}

	private static Dictionary<ExtendedPropertiesKey, Dictionary<string, string>> GetAllExtendedProperties()
	{
		if (allExtendedProperties == null)
		{
			SqlCommand cmd = new SqlCommand(@"DECLARE @result TABLE([Schema] nvarchar(max), [Table] nvarchar(max), [Column] nvarchar(max), [StoredProcedure] nvarchar(max), [ExtendedProperty] nvarchar(max), [Value] nvarchar(max))

-- Extended Properties on tables 
INSERT INTO @result
SELECT [Schemas].[Name], [Tables].[name], '', '', [ExtendedProperties].[name], CONVERT(nvarchar(max), [value]) FROM [sys].[extended_properties] [ExtendedProperties]
INNER JOIN [sys].[tables] [Tables] on ([ExtendedProperties].[major_id] = [Tables].[object_id])
INNER JOIN [sys].[schemas] [Schemas] on [Tables].[schema_id] = [schemas].[schema_id]
WHERE ([ExtendedProperties].[class] = 1) AND ([ExtendedProperties].[minor_id] = 0)

-- Extended Properties on columns
INSERT INTO @result
SELECT [Schemas].[Name], [Tables].[name], [Columns].[name], '', [ExtendedProperties].[name], CONVERT(nvarchar(max), [value]) FROM [sys].[extended_properties] [ExtendedProperties]
INNER JOIN [sys].[tables] [Tables] on ([ExtendedProperties].[major_id] = [Tables].[object_id])
INNER JOIN [sys].[columns] [Columns] on ([Columns].[column_id] = [ExtendedProperties].[minor_id]) AND ([Columns].[object_id] = [ExtendedProperties].[major_id])
INNER JOIN [sys].[schemas] [Schemas] on [Tables].[schema_id] = [schemas].[schema_id]
WHERE ([ExtendedProperties].[class] = 1) AND ([ExtendedProperties].[minor_id] <> 0)

-- Extended Properties on stored Procedures
INSERT INTO @result
SELECT [Schemas].Name, '', '', [StoredProcedures].[name], [ExtendedProperties].[name], CONVERT(nvarchar(max), [value]) FROM [sys].[extended_properties] [ExtendedProperties]
INNER JOIN [sys].[procedures] [StoredProcedures] on ([ExtendedProperties].[major_id] = [StoredProcedures].[object_id])
INNER JOIN [sys].[schemas] [Schemas] on [StoredProcedures].[schema_id] = [schemas].[schema_id]
WHERE ([ExtendedProperties].[class] = 1)

-- Extended Properties on database
INSERT INTO @result
SELECT '', '', '', '', [ExtendedProperties].[name], CONVERT(nvarchar(max), [value]) FROM [sys].[extended_properties] [ExtendedProperties]
WHERE ([ExtendedProperties].[class] = 0)

-- Custom defined extended properties
IF OBJECT_ID('__BLExtendedProperties') IS NOT NULL
INSERT INTO @result
SELECT [Schema], [Table], [Column], [StoredProcedure], [ExtendedProperty], [Value]
FROM __BLExtendedProperties

SELECT * FROM @result");

			List<Tuple<ExtendedPropertiesKey, KeyValuePair<string, string>>> data = new List<Tuple<ExtendedPropertiesKey, KeyValuePair<string, string>>>();
			using (SqlDataReader dataReader = ConnectionHelper.GetDataReader(cmd))
			{
				while (dataReader.Read())
				{
					string schema = dataReader.GetString(0);
					string table = dataReader.GetString(1);
					string column = dataReader.GetString(2);
					string storedProcedure = dataReader.GetString(3);
					string extendedProperty = dataReader.GetString(4);
					string value = dataReader.GetString(5);

					extendedProperty = extendedProperty.Replace("_", ".");

					var key = new ExtendedPropertiesKey(schema, table, column, storedProcedure);
					data.Add(new Tuple<ExtendedPropertiesKey, KeyValuePair<string, string>>(key, new KeyValuePair<string, string>(extendedProperty, value)));
				}
			}

			allExtendedProperties = data.GroupBy(item => item.Item1 /* Key */).ToDictionary(group => group.Key, group => group.ToDictionary(item => item.Item2.Key, item => item.Item2.Value, StringComparer.CurrentCultureIgnoreCase));

		}
		return allExtendedProperties;
	}

	private static Dictionary<ExtendedPropertiesKey, Dictionary<string, string>> allExtendedProperties;

	/// <summary>
	/// Zjišťuje bool hodnotu definovanou Extended Property s klíčem key.
	/// </summary>
	/// <param name="extendedProperties">Extended Properties, kde se extended property hledá.</param>
	/// <param name="key">Klíč extended property.</param>
	/// <param name="location">Kde se nacházíme - jen informační hodnota pro lepší identifikaci chyby (dostane se do výjimky).</param>
	/// <returns>
	///		Extended property nebyla podle klíče nalezena -> null.
	///		Hodnota extended property je "true", "True" nebo "1", pak true.
	///		Hodnota extended property je "false", "False" nebo "0", pak false.
	///		Jinak vyhodí ArgumentException.
	/// </returns>
	public static bool? GetBool(ExtendedPropertiesKey extendedProperties, string key, string location)
	{
		string value = GetString(extendedProperties, key);

		if (value == null)
		{
			return null;
		}

		if (value == "true" || value == "True" || value == "1")
		{
			return true;
		}

		if (value == "false" || value == "False" || value == "0")
		{
			return false;
		}

		throw new ArgumentException(String.Format("Neznámá bool hodnota \"{0}\" v extended property {1} - {2}.", value, location, key));

	}
	/// <summary>
	/// Zjišťuje int hodnotu definovanou Extended Property s klíčem key.
	/// </summary>
	/// <param name="extendedProperties">Extended Properties, kde se extended property hledá.</param>
	/// <param name="key">Klíč extended property.</param>
	/// <param name="location">Kde se nacházíme - jen informační hodnota pro lepší identifikaci chyby (dostane se do výjimky).</param>
	/// <returns>
	///		Extended property nebyla podle klíče nalezena -> null.
	///		Jinak vyhodí ArgumentException.
	/// </returns>
	public static int? GetInt(ExtendedPropertiesKey extendedProperties, string key, string location)
	{
		string value = GetString(extendedProperties, key);

		if (value == null)
		{
			return null;
		}

		if (int.TryParse(value, out int intValue))
		{
			return intValue;
		}

		throw new ArgumentException(String.Format("Neznámá int hodnota \"{0}\" v extended property {1} - {2}.", value, location, key));

	}

	/// <summary>
	/// Vrátí komentář z extended properties.
	/// </summary>
	public static string GetDescription(ExtendedPropertiesKey extendedProperties)
	{
		return GetString(extendedProperties, "MS_Description");
	}
}
