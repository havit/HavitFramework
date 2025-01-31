﻿using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Text;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers;

public static class TableHelper
{
	public static string GetStringExtendedProperty(Table table, string key)
	{
		return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), key);
	}
	public static bool? GetBoolExtendedProperty(Table table, string key)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), key, table.Name);
	}
	public static int? GetIntExtendedProperty(Table table, string key)
	{
		return ExtendedPropertiesHelper.GetInt(ExtendedPropertiesKey.FromTable(table), key, table.Name);
	}

	/// <summary>
	/// Vrací sloupec dle jména.
	/// Vyhledávání je case-insensitive.
	/// </summary>
	public static Column FindColumn(this Table table, string name)
	{
		// Proč nepoužijeme table.Columns[name]?
		// Protože když máme sloupec v databázi pojmenovaný "Abcd", ale dohledáváme "ABCD", pak dostaneme sloupec "ABCD", ačkoliv takto pojmenovaný neexistuje!
		// Na vině je pravděpodobně chování schované někde za tímto řádkem: https://github.com/microsoft/sqlmanagementobjects/blob/3ae2731330f852c8ddc9ab670c6b18cf336d43ab/src/Microsoft/SqlServer/Management/Smo/SmoCollectionBase.cs#L313
		// Proto sloupce načteme a vyhledáme až v načtených sloupcích.
		return table.Columns.Cast<Column>().SingleOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// Vrací true, pokud je tabulka označena jako ignorovaná (Extended Property "Ignored").
	/// Pokud je strategie generování kódu pro Exec, pak se ignorují i všechny tabulky začínající podtržítkem.
	/// </summary>		
	public static bool IsIgnored(Table table)
	{
		return ((GeneratorSettings.Strategy == GeneratorStrategy.Exec) && table.Name.StartsWith("_"))
			|| (GeneratorSettings.Strategy.IsEntityFrameworkGeneratedDatabaseStrategy() && table.Name.StartsWith("__")) // __DataSeed, __EFMigrationsHistory
			|| (table.Name == "__BLExtendedProperties")
			|| (GeneratorSettings.Strategy.IsAnyHavitStrategy() && table.Schema.ToLower().Contains("hangfire"))
			|| (GetBoolExtendedProperty(table, "Ignored") ?? DatabaseHelper.GetDefaultIgnoredOnTables());
	}

	/// <summary>
	/// Vrací true, pokud jde o spojovací tabulku (dekompozice M:N).
	/// Ta se pozná tak, že má právě dva sloupce a oba jsou PFK.
	/// </summary>
	public static bool IsJoinTable(Table table)
	{
		int pfkCounter = 0;
		foreach (Column column in table.Columns)
		{
			if (column.InPrimaryKey && TypeHelper.IsBusinessObjectReference(column) && !ColumnHelper.IsIgnored(column))
			{
				pfkCounter += 1;
			}
		}

		return pfkCounter == 2;
	}

	/// <summary>
	/// Vrátí sloupec, který je primárním klíčem tabulky.
	/// Neni-li PK tvořen právě jedním sloupcem, je vyhozena výjimka.
	/// </summary>
	public static Column GetPrimaryKey(Table table)
	{
		if (getPrimaryKeyCache.TryGetValue(table, out Column result))
		{
			return result;
		}
		else
		{
			foreach (Column column in table.Columns)
			{
				if (!ColumnHelper.IsIgnored(column) && column.InPrimaryKey)
				{
					if (result != null)
					{
						throw new ApplicationException(String.Format("V tabulce '{0}' bylo nalezeno více sloupců, které jsou primárním klíčem.", table.Name));
					}
					result = column;
				}
			}

			if (result == null)
			{
				throw new ApplicationException(String.Format("V tabulce '{0}' nebyl nalezen primární klíč.", table.Name));
			}

			getPrimaryKeyCache[table] = result;
		}
		return result;
	}
	private static Dictionary<Table, Column> getPrimaryKeyCache = new Dictionary<Table, Column>();

	/// <summary>
	/// Vrátí sloupce, kterým se bude generovat property.
	/// Nevrací sloupce primárního klíče a ignorované sloupce.
	/// </summary>
	public static List<Column> GetPropertyColumns(Table table)
	{
		if (lastGetPropertyColumnsTable == table)
		{
			return lastGetPropertyColumnsResult;
		}

		List<Column> result = new List<Column>();
		foreach (Column column in table.Columns.SortIfNecessary())
		{
			if (!column.InPrimaryKey && !ColumnHelper.IsIgnored(column))
			{
				result.Add(column);
			}
		}
		lastGetPropertyColumnsTable = table;
		lastGetPropertyColumnsResult = result;
		return result;
	}
	private static Table lastGetPropertyColumnsTable;
	private static List<Column> lastGetPropertyColumnsResult;

	/// <summary>
	/// Vrací sloupce tabulky bez ignorovaných sloupců.
	/// </summary>
	public static List<Column> GetNotIgnoredColumns(Table table)
	{
		List<Column> result = new List<Column>();
		foreach (Column column in table.Columns.SortIfNecessary())
		{
			if (!ColumnHelper.IsIgnored(column))
			{
				result.Add(column);
			}
		}
		return result;
	}

	/// <summary>
	/// Vrátí sloupce, kterým se bude ukládat hodnota do databáze.
	/// Nevrací sloupce primárního klíče, ignorované sloupce.
	/// </summary>
	public static List<Column> GetDbReadWriteColumns(Table table)
	{
		return GetPropertyColumns(table);
	}

	/// <summary>
	/// Vrátí seznam vlastností typu kolekce, které mají v BusinessObjektu existovat.
	/// Automaticky přidává property Localization, je-li tabulka lokalizována jinou tabulkou.
	/// </summary>
	public static List<CollectionProperty> GetCollectionColumns(Table table)
	{
		if (!_getCollectionColumns.TryGetValue(table, out List<CollectionProperty> collectionColumns))
		{
			collectionColumns = new List<CollectionProperty>();

			foreach (KeyValuePair<string, string> extendedProperty in ExtendedPropertiesHelper.GetTableExtendedProperties(table))
			{
				if (extendedProperty.Key.StartsWith("Collection"))
				{
					string[] parsedName = extendedProperty.Key.Split(new string[] { "_", "." }, StringSplitOptions.None);
					if (parsedName.Length > 2) // ochrana pred Colection_XXXX_Description, apod.
					{
						continue;
					}

					string collectionPropertyName = parsedName[1];
					string extendedPropertyValue = (string)extendedProperty.Value;
					string[] parsedValues = ((string)extendedProperty.Value).Split(new string[] { "." }, StringSplitOptions.None);
					if ((parsedValues.Length < 2) || (parsedValues.Length > 3))
					{
						throw new ApplicationException(String.Format("Při zpracování kolekce '{0}' v tabulce '{1}' se nepodařilo zpracovat hodnotu '{2}'.", collectionPropertyName, table.Name, extendedPropertyValue));
					}

					string collectionTableSchemaName = (parsedValues.Length == 3) ? parsedValues[0] : null;
					string collectionTableName = parsedValues[parsedValues.Length - 2];
					string collectionFieldName = parsedValues[parsedValues.Length - 1];

					Table joinTable = null;
					Table targetTable = (collectionTableSchemaName == null) ? DatabaseHelper.FindTable(collectionTableName, table.Schema, true) : DatabaseHelper.FindTable(collectionTableName, collectionTableSchemaName);
					if (targetTable == null)
					{
						throw new ApplicationException(String.Format("Při zpracování kolekce '{0}' v tabulce '{1}' nebyla nalezena tabulka '{2}'.", collectionPropertyName, table.Name, collectionTableName));
					}
					else if (TableHelper.IsIgnored(targetTable))
					{
						throw new ApplicationException(String.Format("Při zpracování kolekce '{0}' v tabulce '{1}' byla nalezena tabulka '{2}', která je však ignorovaná.", collectionPropertyName, table.Name, collectionTableName));
					}

					Column referenceColumn = targetTable.Columns.Cast<Column>().SingleOrDefault(c => c.Name.Equals(collectionFieldName, StringComparison.OrdinalIgnoreCase));
					if (referenceColumn == null)
					{
						throw new ApplicationException(String.Format("Při zpracování kolekce '{0}' v tabulce '{1}' nebyl nalezen sloupec '{2}' v tabulce '{3}'.", collectionPropertyName, table.Name, collectionFieldName, targetTable.Name));
					}
					if (ColumnHelper.IsIgnored(referenceColumn))
					{
						throw new ApplicationException(String.Format("Při zpracování kolekce '{0}' v tabulce '{1}' byl nalezen ignorovaný sloupec '{2}' v tabulce '{3}'.", collectionPropertyName, table.Name, collectionFieldName, targetTable.Name));
					}

					if (TableHelper.IsJoinTable(targetTable))
					{
						joinTable = targetTable;
						targetTable = TableHelper.GetSecondJoinEnd(targetTable, table);
					}

					string description = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), String.Format("Collection_{0}_Description", collectionPropertyName));
					bool? autoLoadAll = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), String.Format("Collection_{0}_LoadAll", collectionPropertyName), table.Name);
					string sorting = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), String.Format("Collection_{0}_Sorting", collectionPropertyName));
					string propertyAccessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), String.Format("Collection_{0}_PropertyAccessModifier", collectionPropertyName)) ?? ((TableHelper.GetAccessModifier(targetTable) == "internal") ? "internal" : "public");
					bool includeDeleted = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), String.Format("Collection_{0}_IncludeDeleted", collectionPropertyName), table.Name) ?? false;
					CollectionProperty resultItem = new CollectionProperty(table, collectionPropertyName, joinTable, targetTable, referenceColumn, description, autoLoadAll ?? false, propertyAccessModifier, sorting, includeDeleted);
					collectionColumns.Add(resultItem);
				}
			}

			// seřadíme kolekce dle atributů (v praxi se ukazuje, že jsou již seřazené)			
			collectionColumns = collectionColumns.OrderBy(item => item.PropertyName, StringComparer.InvariantCultureIgnoreCase).ToList();

			// abychom neměnili spoustu existujícího kódu, neměníme pořadí lokalizačních tabulek a necháváme je na konci
			if (LocalizationHelper.IsLocalizedTable(table))
			{
				Table targetTable = LocalizationHelper.GetLocalizationTable(table);

				Column referenceColumn = LocalizationHelper.GetParentLocalizationColumn(targetTable);

				if (referenceColumn == null)
				{
					throw new ApplicationException(String.Format("Při zpracování kolekce Localizations v tabulce '{0}' nebyl nalezen sloupec '{1}' v tabulce '{2}'.", table.Name, referenceColumn.Name, targetTable.Name));
				}
				if (ColumnHelper.IsIgnored(referenceColumn))
				{
					throw new ApplicationException(String.Format("Při zpracování kolekce Localizations v tabulce '{0}' byl nalezen ignorovaný sloupec '{1}' v tabulce '{2}'.", table.Name, referenceColumn.Name, targetTable.Name));
				}

				string description = "Lokalizované hodnoty.";

				CollectionProperty localizationProperty = new CollectionProperty(table, "Localizations", null, targetTable, referenceColumn, description, false, "public", null, false);
				collectionColumns.Add(localizationProperty);
			}

			_getCollectionColumns.Add(table, collectionColumns);
		}
		return collectionColumns;
	}
	private static Dictionary<Table, List<CollectionProperty>> _getCollectionColumns = new Dictionary<Table, List<CollectionProperty>>();

	/// <summary>
	/// Vrátí seznam vlastností typu kolekce, které mají v BusinessObjektu existovat.
	///
	/// </summary>
	public static List<CollectionProperty> GetCollectionsTargetingTo(Table table)
	{
		List<CollectionProperty> result = new List<CollectionProperty>();
		foreach (Table workingTable in DatabaseHelper.GetWorkingTables())
		{
			foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(workingTable))
			{
				if (collectionProperty.TargetTable == table)
				{
					result.Add(collectionProperty);
				}
			}
		}
		return result;
	}

	/// <summary>
	/// Spojka (spojovací tabulka) má dva klíče, které referují dvě tabulky.
	/// Tato metoda najde ke spojce a tabulce druhou tabulku, která je referována.
	/// </summary>
	public static Table GetSecondJoinEnd(Table joinTable, Table firstEndTable)
	{
		foreach (ForeignKey foreignKey in joinTable.ForeignKeys)
		{
			Table resultTable = DatabaseHelper.FindTable(foreignKey.ReferencedTable, foreignKey.ReferencedTableSchema);
			if (firstEndTable != resultTable)
			{
				return resultTable;
			}
		}

		// pokud je referována jen jedna tabulka (např. návrhovy vzor Composit), musíme ji vrátit.
		return firstEndTable;
	}

	/// <summary>
	/// Vrátí druhý sloupec v tabulce vůči zadanému sloupci. Vyžaduje se, aby tabulka měla právě 2 sloupečky.
	/// </summary>
	public static Column GetSecondColumn(Table table, Column firstColumn)
	{
		int counter = 0;
		foreach (Column column in table.Columns)
		{
			if (!ColumnHelper.IsIgnored(column))
			{
				counter += 1;
			}
		}

		if (counter != 2)
		{
			throw new ArgumentException("Tabulka nemá očekávaný počet sloupců, předpokládají se dva.");
		}

		foreach (Column column in table.Columns)
		{
			if ((!ColumnHelper.IsIgnored(column)) && (column != firstColumn))
			{
				return column;
			}
		}

		throw new ApplicationException("Chyba v programu.");
	}

	/// <summary>
	/// Udává, zda je tabulka určena jen pro čtení.
	/// Výchozí hodnota je false.
	/// Výjimkou je tabulka Language, jejíž výchozí hodnota je true.
	/// </summary>
	public static bool IsReadOnly(Table table)
	{
		bool? readonlyValue = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "ReadOnly", table.Name);

		if (readonlyValue != null)
		{
			return readonlyValue.Value;
		}

		if (LocalizationHelper.IsLocalizationTable(table))
		{
			return TableHelper.IsReadOnly(LocalizationHelper.GetLocalizationParentTable(table));
		}

		if (LanguageHelper.IsLanguageTable(table))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Vrátí typ výčtu generovaného pro tabulku.
	/// </summary>
	public static EnumMode GetEnumMode(Table table)
	{
		switch (ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "EnumMode"))
		{
			case "EnumClass": return EnumMode.EnumClass;
			default: return EnumMode.NoEnum;
		}
	}

	/// <summary>
	/// Vrátí seznam členů výčtu k tabulce.
	/// </summary>
	public static List<EnumMember> GetEnumMembers(Table table)
	{
		List<EnumMember> result = new List<EnumMember>();

		string idColumn = TableHelper.GetPrimaryKey(table).Name;

		string nameColumn;

		nameColumn = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "EnumPropertyNameField");
		if (nameColumn != null)
		{
			if (table.FindColumn(nameColumn) == null)
			{
				throw new ApplicationException(
					String.Format("Sloupec '{0}' nebyl v tabulce '{1}' nalezen (EnumPropertyNameField).", nameColumn, table.Name));
			}
		}
		if (nameColumn == null)
		{
			nameColumn = ColumnHelper.FindFirstExistingColumn(table, "PropertyName", "Symbol", "Nazev", "Name");
		}

		if (nameColumn == null)
		{
			throw new ApplicationException("Nepodařilo se určit název sloupce pro název property výčtu.");
		}

		string commentColumn = ColumnHelper.FindFirstExistingColumn(table, "Komentar", "Comment");

		using (
			SqlCommand command =
				new SqlCommand(
					String.Format("SELECT [{2}], [{3}]{4} FROM [{0}].[{1}] WHERE LEN([{3}]) > 0",
						table.Schema, // 0
						table.Name, // 1
						idColumn, // 2
						nameColumn, // 3
						String.IsNullOrEmpty(commentColumn) ? "" : ", [" + commentColumn + "]"))) // 4				
		{
			using (SqlDataReader reader = ConnectionHelper.GetDataReader(command))
			{
				while (reader.Read())
				{
					int id = (int)reader[idColumn];

					string name = (string)reader[nameColumn];
					string[] names = name.Trim().Split(' ');
					for (int i = 0; i < names.Length; i++)
					{
						names[i] = names[i].Substring(0, 1).ToUpper() + names[i].Substring(1);
					}
					name = String.Join("", names);

					string comment = null;
					if (commentColumn != null)
					{
						comment = (string)reader[commentColumn];
					}

					result.Add(new EnumMember(id, name, comment));
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Vrátí fieldy do sekce SQL dotazu SELECT a to včetně kolekcí.
	/// </summary>
	public static object GetSqlSelectFields(Table table, bool collectionQueryBySqlParameter)
	{
		StringBuilder fieldsBuilder = new StringBuilder();
		bool wasFirst = false;
		foreach (Column column in TableHelper.GetNotIgnoredColumns(table))
		{
			if (wasFirst)
			{
				fieldsBuilder.Append(", ");
			}
			fieldsBuilder.AppendFormat("[{0}]", column.Name);
			wasFirst = true;
		}

		foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
		{
			if (wasFirst)
			{
				fieldsBuilder.Append(", ");
			}
			fieldsBuilder.AppendFormat(ColumnHelper.GetSqlSelectFieldStatementForCollectionProperty(table, collectionProperty, collectionQueryBySqlParameter));
			wasFirst = true;
		}
		return fieldsBuilder.ToString();
	}

	/// <summary>
	/// Vrátí true, pokud se mají záznamy z tabulky cachovat.
	/// Výchozí hodnota je false.
	/// Výjimkou je tabulka Language, jejíž výchozí hodnota je true.
	/// </summary>
	public static bool IsCachable(Table table)
	{
		bool? isCachable = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "Cache", table.Name);
		if (isCachable != null)
		{
			return isCachable.Value;
		}

		if (LocalizationHelper.IsLocalizationTable(table))
		{
			return TableHelper.IsCachable(LocalizationHelper.GetLocalizationParentTable(table));
		}

		if (LanguageHelper.IsLanguageTable(table))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Vrací true, pokud mohou být cachovány celé instance business objektů (pro cachované readonly tabulky, které neodkazují do non-readonly tabulky (netranzitivně)).
	/// V opačném případě jsou cachovány DataRecords.
	/// </summary>
	public static bool CanCacheBusinessObjectInstances(Table table)
	{
		if (!IsCachable(table) || !IsReadOnly(table))
		{
			return false; // tabulka není cachovaná (nebo readonly), nemůžeme cachovat instance
		}

		HashSet<Table> referencedTablesIncludingSelf = new HashSet<Table>();
		// získáme všechny závislé tabulky (vč. tabulky samotné) a to včetně tranzitivních závislostí (tedy závislostí závislostí)
		CanCacheBusinessObjectInstances_AddReferencedTables(referencedTablesIncludingSelf, table);

		// pokud jsou všechny (tranzitivně) referencované tabulky read-only, můžeme cachovat instance
		return referencedTablesIncludingSelf.All(referencedTable => TableHelper.IsReadOnly(referencedTable));
	}

	/// <summary>
	/// Přidá do hashsetu tabulku a všechny její tranzitivní závislosti (tabulky referované cizími klíči a tabulky referované kolekcemi).
	/// </summary>
	private static void CanCacheBusinessObjectInstances_AddReferencedTables(HashSet<Table> referencedTables, Table table)
	{
		if (referencedTables.Contains(table))
		{
			// už jsme tu byli, chráníme se před cyklem (nic neobvyklého)
			return;
		}

		// zaznamenáme si tabulku
		referencedTables.Add(table);

		// a projdeme všechny její závislosti
		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			if (TypeHelper.IsBusinessObjectReference(column))
			{
				Table referencedTable = ColumnHelper.GetReferencedTable(column);
				CanCacheBusinessObjectInstances_AddReferencedTables(referencedTables, referencedTable);
			}
		}

		foreach (CollectionProperty collection in TableHelper.GetCollectionColumns(table))
		{
			CanCacheBusinessObjectInstances_AddReferencedTables(referencedTables, collection.TargetTable);
		}
	}

	/// <summary>
	/// Vrátí prioritu s jakou se cachují záznamy tabulky.
	/// </summary>
	public static string GetCachePriority(Table table)
	{
		return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "Cache_Priority");
	}

	/// <summary>
	/// Vrátí délku absolutní expirace pro cachování záznamů tabulky v sekundách.
	/// </summary>
	public static int? GetCacheAbsoluteExpirationSeconds(Table table)
	{
		string expiration = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "Cache_AbsoluteExpiration");
		if (String.IsNullOrEmpty(expiration))
		{
			return null;
		}
		else
		{
			return int.Parse(expiration);
		}
	}

	/// <summary>
	/// Vrátí délku sliding expirace pro cachování záznamů tabulky v sekundách.
	/// </summary>
	public static int? GetCacheSlidingExpirationSeconds(Table table)
	{
		string expiration = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "Cache_SlidingExpiration");
		if (String.IsNullOrEmpty(expiration))
		{
			return null;
		}
		else
		{
			return int.Parse(expiration);
		}
	}

	/// <summary>
	/// Nalezne v tabulce sloupec identifikující příznakem smazané záznamy.
	/// Pokud není sloupec nalezen, vrací false.
	/// </summary>
	public static Column GetDeletedColumn(Table table)
	{
		foreach (Column column in table.Columns)
		{
			if (ColumnHelper.IsDeletedColumn(column)) // jen neignorované
			{
				return column;
			}
		}
		return null;
	}

	/// <summary>
	/// Vrátí sloupce, které určují ownera (ownery) záznamu.
	/// V tabulce musí být extended property OwnerField s názvem sloupce, který nese hodnotu ownera.
	/// Příklad použití: Ve vztahu Faktura-PolozkaFaktury vrátí pro tabulku PolozkaFaktury sloupec FakturaID, který je cizím klíčem do tabulky faktura.
	/// </summary>
	public static List<Column> GetOwnerColumns(Table table)
	{
		List<Column> result = new List<Column>();

		if (LocalizationHelper.IsLocalizationTable(table))
		{
			result.Add(LocalizationHelper.GetParentLocalizationColumn(table));
			result.Add(table.FindColumn(LocalizationHelper.LanguageForeignKeyColumnName));

			return result;
		}

		string ownerFields = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "OwnerFields");

		if (ownerFields == null)
		{
			ownerFields = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "OwnerField");
		}

		if (ownerFields == null)
		{
			ownerFields = "";
		}

		foreach (string field in ownerFields.Trim().Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
		{
			string fieldName = field.Trim();
			Column column = table.FindColumn(fieldName);

			if (column == null)
			{
				throw new ApplicationException(String.Format("Sloupec {0} definovaný jako owner field v tabulce {1} nebyl nalezen.", fieldName, table.Name));
			}

			result.Add(column);
		}
		return result;
	}

	/// <summary>
	/// Vrátí desctiption k tabulce, 
	/// pro tabulku "Language" a lokalizační tabulky vrací výchozí hodnotu, pokud description neexistuje.
	/// </summary>
	public static string GetDescription(Table table, bool suppressDefaults = false)
	{
		string description = ExtendedPropertiesHelper.GetDescription(ExtendedPropertiesKey.FromTable(table));

		if (description != null)
		{
			return description;
		}

		if (!suppressDefaults)
		{
			if (LanguageHelper.IsLanguageTable(table))
			{
				return "Jazyk (lokalizace).";
			}

			if (LocalizationHelper.IsLocalizationTable(table))
			{
				return String.Format("Lokalizace objektu {0}.", ClassHelper.GetClassFullName(LocalizationHelper.GetLocalizationParentTable(table)));
			}
		}

		return null;
	}

	/// <summary>
	/// Určuje, zda se k tabulce mají tvořit indexy.
	/// </summary>
	public static bool GetGenerateIndexes(Table table)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "GenerateIndexes", table.Name) ?? true;
	}

	/// <summary>
	/// Vrací hodnotu extended property GetAll_IncludeLocalizations tabulky. Není-li uvedena, vrací výchozí nastavení databáze.
	/// </summary>
	public static bool GetGetAllIncludeLocalizations(Table table)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "GetAll_IncludeLocalizations", table.Name)
			?? DatabaseHelper.GetGetAllIncludeLocalizations();
	}

	/// <summary>
	/// Vrací hodnotu extended property GetAll_IncludeLocalizations tabulky. Není-li uvedena, vrací výchozí nastavení databáze.
	/// </summary>
	public static bool GetLoadAllIncludeLocalizations(Table table)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "LoadAll_IncludeLocalizations", table.Name)
			?? DatabaseHelper.GetLoadAllIncludeLocalizations();
	}

	/// <summary>
	/// Vrátí přístupový modifikátor pro třídu generovanou k tabulce.
	/// </summary>
	public static string GetAccessModifier(Table table)
	{
		string accessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "AccessModifier");
		if (!String.IsNullOrEmpty(accessModifier))
		{
			return accessModifier;
		}

		if (LocalizationHelper.IsLocalizationTable(table))
		{
			return TableHelper.GetAccessModifier(LocalizationHelper.GetLocalizationParentTable(table));
		}

		return "public";

	}

	/// <summary>
	/// Vrací přístupový modifikátor pro metodu CreateObject.
	/// Čte extended property CreateObjectAccessModifier.
	/// Výchozí hodnota je "public".
	/// </summary>
	public static string GetCreateObjectAccessModifier(Table table)
	{
		string modifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "CreateObjectAccessModifier");
		if (String.IsNullOrEmpty(modifier))
		{
			return "public";
		}
		return modifier;
	}

	/// <summary>
	/// Vrací true, pokud má být vynechána metoda CreateObject (true -> nemá být generována).
	/// </summary>
	public static bool OmitCreateObjectMethod(Table table)
	{
		return GetCreateObjectAccessModifier(table).ToLower() == "none";
	}

	/// <summary>
	/// Vrátí celý název databázové tabulky vč. názvu schématu, pokud je schéma v cílové platformě podporováno.
	/// Pokud není podporováno, vrací jen název tabulky.
	/// </summary>
	public static string GetFullTableName(Table table)
	{
		if (DatabaseHelper.IsDatabaseSchemaSupported())
		{
			return String.Format("[{0}].[{1}]", table.Schema, table.Name);
		}
		else
		{
			return String.Format("[{0}]", table.Name);
		}
	}

	/// <summary>
	/// Vrátí příznak, zda se má generovat method pro klonování objektu.
	/// </summary>
	public static bool GetCloneMethod(Table table)
	{
		return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable(table), "CloneMethod", table.Name) ?? false;
	}

	/// <summary>
	/// Vrátí přístupový modifikátor pro danou třídu.
	/// </summary>
	public static string GetCloneMethodAccessModifier(Table table)
	{
		string methodAccessModifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "CloneMethodAccessModifier");
		if (String.IsNullOrEmpty(methodAccessModifier))
		{
			methodAccessModifier = "protected internal";
		}

		return methodAccessModifier;
	}

	public static List<String> Script(Table table)
	{
		if (_tableScripts == null)
		{
			ScriptingOptions scriptingOptions = new ScriptingOptions();
			scriptingOptions.BatchSize = 20;
			scriptingOptions.AnsiPadding = false;
			scriptingOptions.AgentJobId = false;
			scriptingOptions.DriAll = false;
			scriptingOptions.DriChecks = true;
			scriptingOptions.DriDefaults = true;
			scriptingOptions.DriForeignKeys = true;
			scriptingOptions.DriPrimaryKey = true;
			scriptingOptions.DriUniqueKeys = true;
			scriptingOptions.DriWithNoCheck = true;
			scriptingOptions.Statistics = false;
			scriptingOptions.TargetServerVersion = SqlServerVersion.Version140;

			Urn[] tableUrns = DatabaseHelper.GetWorkingTables().Select(item => item.Urn).ToArray();
			Server server = DatabaseHelper.Database.Parent;
			Scripter scripter = new Scripter(server);

			scripter.Options = scriptingOptions;
			StringCollection scripts = scripter.Script(tableUrns);
			_tableScripts = scripts.Cast<string>().Where(line => line.StartsWith("CREATE TABLE") || line.StartsWith("ALTER TABLE")).ToList();
		}

		string createTable = $"CREATE TABLE [{table.Schema}].[{table.Name}]";
		string alterTable = $"ALTER TABLE [{table.Schema}].[{table.Name}]";
		return _tableScripts.Where(line => line.StartsWith(createTable) || line.StartsWith(alterTable)).ToList();
	}
	private static List<string> _tableScripts;

	internal static List<Column> SortIfNecessary(this ColumnCollection columnCollection)
	{
		return columnCollection.Cast<Column>().ToList().SortIfNecessary();
	}

	internal static List<Column> SortIfNecessary(this List<Column> columnCollection)
	{
		List<Column> result = columnCollection.Cast<Column>().ToList();
		// Pro HavitCodeFirst strategii budeme generovat sloupce v abecedním pořadí (s výjimkou PK), abychom omezili vznik konfliktů a udělali generování s EF Core Migrations determinističtější.
		if (GeneratorSettings.Strategy.IsEntityFrameworkGeneratedDatabaseStrategy())
		{
			var pkColumnsInReverseOrder = result.Where(column => column.InPrimaryKey).ToList();
			pkColumnsInReverseOrder.ForEach(pkColumn => result.Remove(pkColumn)); // sloupce s PK se neúčastní řazení

			result = result.OrderBy(column => column.Name, StringComparer.InvariantCultureIgnoreCase).ToList(); // seřadíme sloupce abecedně

			// vložíme zpět sloupce s PK
			pkColumnsInReverseOrder.Reverse();
			pkColumnsInReverseOrder.ToList().ForEach(pkColumn => result.Insert(0, pkColumn)); // protože můžeme vkládat více sloupců a vkládáme je na pozici 0, vkládáme je v opačném pořadí (Reverse)
		}

		return result;
	}
}
