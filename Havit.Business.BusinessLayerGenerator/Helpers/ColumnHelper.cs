using System;
using System.Data;
using System.Text.RegularExpressions;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class ColumnHelper
	{
		public static string GetStringExtendedProperty(Column column, string key)
		{
			return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), key);
		}
		public static bool? GetBoolExtendedProperty(Column column, string key)
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromColumn(column), key, ((Table)column.Parent).Name + "-" + column.Name);
		}
		public static int? GetIntExtendedProperty(Column column, string key)
		{
			return ExtendedPropertiesHelper.GetInt(ExtendedPropertiesKey.FromColumn(column), key, ((Table)column.Parent).Name + "-" + column.Name);
		}

		#region IsIgnored
		/// <summary>
		/// Vrací true, pokud je sloupec označena jako ignorovaný (Extended Property "Ignored").
		/// </summary>
		public static bool IsIgnored(Column column)
		{
			if ((GeneratorSettings.Strategy == GeneratorStrategy.Exec) && column.DataType.SqlDataType == SqlDataType.Timestamp)
			{
				return true;
			}

			return (column.Name == "PropertyName")
				|| (GetBoolExtendedProperty(column, "Ignored") ?? false);
		}
		#endregion

		#region IsReadOnly
		/// <summary>
		/// Vrací true, pokud je sloupec označena jako ReadOnly (Extended Property "ReadOnly").
		/// </summary>
		public static bool IsReadOnly(Column column)
		{
			return (GetBoolExtendedProperty(column, "ReadOnly") ?? (column.Name == "Created"));
		}
		#endregion

		#region GetReferencedTable
		/// <summary>
		/// Vrací tabulku, kterou referencuje cizí klíč.
		/// Není-li sloupec FK, vyhodí výjimku.
		/// </summary>
		public static Table GetReferencedTable(Column column)
		{
			ForeignKey foreignKey = GetForeignKey(column);

			return DatabaseHelper.FindTable(foreignKey.ReferencedTable, foreignKey.ReferencedTableSchema, includeIgnored: true);
		}
		#endregion

		#region GetForeignKey
		/// <summary>
		/// Vrátí FK daného stĺpca.
		/// Není-li sloupec FK, vyhodí výjimku.
		/// </summary>
		public static ForeignKey GetForeignKey(Column column)
		{
			if (!TypeHelper.IsBusinessObjectReference(column))
			{
				throw new ArgumentException(String.Format("Sloupec \"{0}\" v tabulce \"{1}\" není referencí.", column.Name, ((Table)column.Parent).Name));
			}

			Table ownerTable = (Table)column.Parent;

			foreach (ForeignKey foreignKey in ownerTable.ForeignKeys)
			{
				if (foreignKey.Columns.Count == 1 && foreignKey.Columns[0].Name == column.Name)
				{
					return foreignKey;
				}
			}

			throw new ApplicationException(String.Format("Tabulka {0}, Sloupec {1}: Referovanou tabulku se nepodařilo nalést.", ownerTable.Name, column.Name));
		}
		#endregion

		#region GetReferencedColumn

		/// <summary>
		/// Vrátí referencovaný stĺpec z cieľovej tabuľky.
		/// Není-li sloupec FK, vyhodí výjimku.
		/// </summary>
		public static Column GetReferencedColumn(Column column)
		{
			ForeignKey foreignKey = GetForeignKey(column);

			Table table = DatabaseHelper.FindTable(foreignKey.ReferencedTable, foreignKey.ReferencedTableSchema, includeIgnored: true);

			return table.Columns[foreignKey.Columns[0].ReferencedColumn];
		}

		#endregion

		#region GetParameterValue
		/// <summary>
		/// Vrátí řetězec, kterým se získává hodnota pro SqlParameter z hodnoty property k danému sloupci.
		/// </summary>
		public static string GetParameterValue(Column column)
		{
			// pokud jde o nestandarní datový typ, pokužijeme konverzi (pokud existuje)
			if (TypeHelper.IsNonstandardType(column))
			{
				string typeConverter = TypeHelper.GetNonstandarPropertyTypeConverterName(column);
				if (!String.IsNullOrEmpty(typeConverter))
				{
					return String.Format("{0}.ConvertTo({1}.Value, typeof({2})) ?? DBNull.Value", ConverterHelper.GetFieldConvertorName(column), PropertyHelper.GetPropertyHolderName(column), TypeHelper.GetFieldSystemTypeName(column));
				}
			}

			// pokud jde o XML, předáme jako řetězec, funguje to
			// sice se všude píše o SqlXml, ale toto jde taky a navíc velmi jednoduše
			// předpokládáme použití "normálně" velkých dokumentů
			if (TypeHelper.IsXml(column))
			{
				return String.Format("{0}.Value == null ? DBNull.Value : (object){0}.Value.OuterXml", PropertyHelper.GetPropertyHolderName(column));
			}

			// speciální workaround pro ukládání stringu jako XmlDocumentu
			if ((TypeHelper.GetFieldSystemTypeName(column) == "XmlDocument") && PropertyHelper.IsString(column))
			{
				return String.Format("String.IsNullOrEmpty({0}.Value) ? DBNull.Value : (object){0}.Value", PropertyHelper.GetPropertyHolderName(column));				
			}

			// ale pokud jde o retezec, tak neukládáme jako null, ale jako prázdný řetězec
			if (PropertyHelper.IsString(column))
			{
				return String.Format("{0}.Value ?? String.Empty", PropertyHelper.GetPropertyHolderName(column));
			}

			// a pokud jde o cizí klíč, uložíme hodnotu cizího klíče.
			if (TypeHelper.IsBusinessObjectReference(column))
			{
				return String.Format("({0}.Value == null) ? DBNull.Value : (object){0}.Value.ID", PropertyHelper.GetPropertyHolderName(column));
			}

			// pokud sloupec povoluje null, umožníme uložení hodnoty null
			if (column.Nullable)
			{
				return String.Format("({0}.Value == null) ? DBNull.Value : (object){0}.Value", PropertyHelper.GetPropertyHolderName(column));
			}

			// hodnota parametru je hodnotou property holderu
			return String.Format("{0}.Value", PropertyHelper.GetPropertyHolderName(column));
		}
		#endregion

		#region FindFirstExistingColumn
		/// <summary>
		/// Vrátí ze seznamu názvů sloupců první sloupec, který existuje v tabulce. Není-li žádný nalezen, vrátí null.
		/// </summary>
		public static string FindFirstExistingColumn(Table table, params string[] columnNames)
		{
			foreach (string columnName in columnNames)
			{
				if (table.Columns[columnName] != null)
				{
					return columnName;
				}
			}
			return null;
		}
		#endregion

		#region GetSqlSelectFieldStatementForCollectionProperty
		/// <summary>
		/// Vrátí field do sekce SQL dotazu SELECT pro danou kolekci.
		/// </summary>
		public static string GetSqlSelectFieldStatementForCollectionProperty(Table table, CollectionProperty collectionProperty, bool collectionQueryBySqlParameter)
		{
			// název sloupečku obsahující ID, která nás zajímají
			string fieldName = collectionProperty.IsManyToMany ? TableHelper.GetSecondColumn(collectionProperty.JoinTable, collectionProperty.ReferenceColumn).Name : TableHelper.GetPrimaryKey(collectionProperty.TargetTable).Name;

			// schéma a tabulka obsahující referenční sloupeček
			//string schemaName = collectionProperty.JoinTable != null ? collectionProperty.JoinTable.Schema : collectionProperty.TargetTable.Schema;
			//string tableName = collectionProperty.JoinTable != null ? collectionProperty.JoinTable.Name : collectionProperty.TargetTable.Name;
			Table joinTable = collectionProperty.IsManyToMany ? collectionProperty.JoinTable : collectionProperty.TargetTable;

			// s čím se porovnává ID referencovaného sloupce
			string collectionParent = collectionQueryBySqlParameter ? "@" + TableHelper.GetPrimaryKey(table).Name : String.Format("{0}.[{1}]", TableHelper.GetFullTableName(table), TableHelper.GetPrimaryKey(table).Name);

			Column deletedColumn = TableHelper.GetDeletedColumn(collectionProperty.TargetTable);

			// různé vlastnosti dotazu
			bool isOneToMany = collectionProperty.IsOneToMany;
			bool usesDeletedRestriction = (deletedColumn != null) && (!collectionProperty.IncludeDeleted);
			bool usesSorting = !String.IsNullOrEmpty(collectionProperty.Sorting);

			// pořadí
			string orderby = "";
			if (usesSorting)
			{
				orderby = " ORDER BY " + Regex.Replace(collectionProperty.Sorting, "(^|[^{]){([^{}]*)}", delegate(Match match)
					{
						return match.Groups[1] + "[_members].[" + match.Groups[2] + "]";
					});
			}

			// smazaný záznam
			string notDeletedConstraint = "";
			if (usesDeletedRestriction)
			{
				if (deletedColumn.DataType.SqlDataType == SqlDataType.Bit)
				{
					notDeletedConstraint = String.Format(" and ([_members].[{0}] = 0)", deletedColumn.Name);
				}
				if (TypeHelper.IsDateTime(deletedColumn))
				{
					notDeletedConstraint = String.Format(" and ([_members].[{0}] IS NULL)", deletedColumn.Name);
				}
			}

			if (!usesDeletedRestriction && !usesSorting)
			{
				// nemáme sloupeček identifikující smazané záznamy, ani určeno pořadí záznamů
				// o nic se nestaráme, prostě vytáhneme ID.

				// poznámka: ve vazbě M:N prostě ze spojky vytáhneme IDčka, cílová tabulka nás vůbec nezajímá.
				return String.Format(
					"(SELECT CAST([_items].[{0}] AS NVARCHAR(11)) + '|' FROM {1} AS [_items] WHERE ([_items].[{2}] = {3}) FOR XML PATH('')) AS [{4}]",
					fieldName, // 0
					TableHelper.GetFullTableName(joinTable), // 1
					collectionProperty.ReferenceColumn.Name, // 2
					collectionParent, // 3
					collectionProperty.PropertyName); //4
			}

			// máme test na smazané záznamy NEBO určeno pořadí...

			if (isOneToMany)
			{
				// máme sloupeček identifikující smazané záznamy NEBO určeno pořadí
				// A ZÁROVEŇ vztah 1:N			
				return String.Format("(SELECT CAST([_members].[{0}] AS NVARCHAR(11)) + '|' FROM {1} AS [_members] WHERE ([_members].[{2}] = {3}){4} {5} FOR XML PATH('')) AS [{6}]",
					fieldName, // 0
					TableHelper.GetFullTableName(joinTable), // 1
					collectionProperty.ReferenceColumn.Name, // 2
					collectionParent, // 3
					notDeletedConstraint, // 4
					orderby, // 5
					collectionProperty.PropertyName); // 6
			}
			else
			{
				// máme sloupeček identifikující smazané záznamy nebo určeno pořadí
				// A ZÁROVEŇ vztah M:N
				return String.Format("(SELECT CAST([_joins].[{0}] AS NVARCHAR(11)) + '|' FROM {1} AS [_joins] INNER JOIN {2} AS [_members] ON (([_joins].[{0}] = [_members].[{3}]){4}) WHERE ([_joins].[{5}] = {6}){7} FOR XML PATH('')) AS [{8}]",
					fieldName, // 0
					TableHelper.GetFullTableName(collectionProperty.JoinTable), //1
					TableHelper.GetFullTableName(collectionProperty.TargetTable), // 2
					TableHelper.GetPrimaryKey(collectionProperty.TargetTable).Name, // 3
					notDeletedConstraint, // 4
					collectionProperty.ReferenceColumn.Name, // 5
					collectionParent, // 6
					orderby, // 7
					collectionProperty.PropertyName); // 8
			}

		}
		#endregion

		#region IsDeprecatedType
		/// <summary>
		/// Vrátí true, pokud je předán sloupec, jehož typ je zastaralý.
		/// </summary>
		public static bool IsDeprecatedType(Column column)
		{
			return column.DataType.SqlDataType == SqlDataType.Image
				|| column.DataType.SqlDataType == SqlDataType.NText
				|| column.DataType.SqlDataType == SqlDataType.Text;
		}
		#endregion

		#region IsDeletedColumn
		/// <summary>
		/// Vrací true, pokud je sloupec identifikuje příznakem smazaný záznam.
		/// </summary>
		public static bool IsDeletedColumn(Column column)
		{
			return (column.Name == "Deleted")
				&& ((column.DataType.SqlDataType == SqlDataType.Bit) || TypeHelper.IsDateTime(column))
				&& (!ColumnHelper.IsIgnored(column));
		}
		#endregion

		#region GetDescription
		/// <summary>
		/// Vrátí popis sloupce dle databáze nebo výchozí hodnotu.
		/// </summary>
		public static string GetDescription(Column column)
		{            
			string description = ExtendedPropertiesHelper.GetDescription(ExtendedPropertiesKey.FromColumn(column));

			if (!String.IsNullOrEmpty(description))
			{
				return description;
			}

			if (column.InPrimaryKey)
			{
				return "Identifikátor objektu.";
			}

			Table table = (Table)column.Parent;

			if ((PropertyHelper.GetPropertyName(column) == "Culture") && LanguageHelper.IsLanguageTable(table))
			{
				return "Název culture. Zpravidla pětipísmený kód, např. cs-CZ, en-US, apod.";
			}

			if (LocalizationHelper.IsLocalizationTable(table))
			{
				if (column.Name == LocalizationHelper.LanguageForeignKeyColumnName)
				{
					return "Jazyk lokalizovaných dat.";
				}
				if (column.Name == TableHelper.GetPrimaryKey(LocalizationHelper.GetLocalizationParentTable(table)).Name)
				{
					return "Lokalizovaný objekt.";
				}
			}

			if (MoneyHelper.FormsMoneyStructure(column) && (column == MoneyHelper.GetMoneyCurrencyColumn(table, MoneyHelper.ShortcutColumnNameToMoneyPropertyName(column.Name))))
			{
				Column amountColumn = MoneyHelper.GetMoneyAmountColumn(table, MoneyHelper.ShortcutColumnNameToMoneyPropertyName(column.Name));
				string amountDescription = ColumnHelper.GetDescription(amountColumn);
				if (!String.IsNullOrEmpty(amountDescription))
				{
					amountDescription += " [Měna]";
					return amountDescription;
				}
			}

			if (column.Name == "Nazev")
			{
				return "Název.";
			}

			if (IsDeletedColumn(column) && column.DataType.SqlDataType == SqlDataType.Bit)
			{
				return "Indikuje smazaný záznam.";
			}

			if (IsDeletedColumn(column))
			{
				return "Čas smazání objektu.";
			}

			if (column.Name == "Created")
			{
				return "Čas vytvoření objektu.";
			}

			if (column.Name == "CreatedBy")
			{
				return "Zakladatel objektu.";
			}

			if (column.Name == "Symbol")
			{
				return "Symbol.";
			}

			return null;
		}
		#endregion

		#region IsStringTrimming
		/// <summary>
		/// Udává, zda se má na sloupci provádět string trimming.
		/// </summary>
		public static bool IsStringTrimming(Column column)
		{
			Table table = (Table)column.Parent;
			bool? stringTrimming = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromColumn(column), "StringTrimming", table.Name + " - " + column.Name);

			if (stringTrimming == null)
			{
				return DatabaseHelper.IsStringTrimming();
			}
			else
			{
				return stringTrimming.Value;
			}
		}
		#endregion

		#region IsForeignKeyCheckSuppressed
		public static bool CheckForeignKeyName(Column column)
		{
			if ((GeneratorSettings.Strategy != GeneratorStrategy.Havit) && (GeneratorSettings.Strategy != GeneratorStrategy.HavitCodeFirst))
			{
				return false;
			}

			bool? columnSuppressForeignKeyCheck = ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromColumn(column), "CheckForeignKeyName", ((Table)column.Parent).Name + "-" + column.Name);
			if (columnSuppressForeignKeyCheck != null)
			{
				 return columnSuppressForeignKeyCheck.Value;
			}

			if ((column.Name == "BusinessObjectID") || (column.Name.StartsWith("External")))
			{
				return false;
			}

			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromTable((Table)column.Parent), "CheckForeignKeyName", ((Table)column.Parent).Name) ?? true;
		}
		#endregion

		#region GetGenerateIndexes
		/// <summary>
		/// Určuje, zda se ke sloupci mají tvořit indexy.
		/// </summary>
		public static bool GetGenerateIndexes(Column column)
		{
			return ExtendedPropertiesHelper.GetBool(ExtendedPropertiesKey.FromColumn(column), "GenerateIndexes", ((Table)column.Parent).Name + "-" + column.Name) ?? true;
		}
		#endregion

		#region GetColumnMaximumLength
		/// <summary>
		/// Vrátí "délku" dat sloupce.
		/// Pro sloupce "neomezené" délky vrací  2^31 - 1.
		/// </summary>
		public static int GetColumnMaximumLength(Column column)
		{
			if (IsLengthUnlimitedTextColumn(column))
			{
				checked
				{
					return (int)(Math.Pow(2, 31) - 1);
				}
			}
			return column.DataType.MaximumLength;
		}
		#endregion

		#region IsLengthUnlimitedTextColumn
		/// <summary>
		/// Vrátí true, pokud je typ VarCharMax, NVarCharMax, VarBinaryMax, Text nebo NText.
		/// </summary>
		public static bool IsLengthUnlimitedTextColumn(Column column)
		{
			// Exec má po svém pojmenované sloupce...
			if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
			{
				if (column.DataType.MaximumLength == -1)
				{
					return true;
				}
			}

			return column.DataType.SqlDataType == SqlDataType.VarCharMax
				|| column.DataType.SqlDataType == SqlDataType.NVarCharMax
				|| column.DataType.SqlDataType == SqlDataType.VarBinaryMax
				|| column.DataType.SqlDataType == SqlDataType.Text
				|| column.DataType.SqlDataType == SqlDataType.NText;
		}
		#endregion

		#region GetCloneMode
		/// <summary>
		/// Vrátí mód klonování property.
		/// Vrací hodnotu dle extended property "CloneMode". Výchozí hodnotou je CloneMode.Shallow.
		/// </summary>
		public static CloneMode GetCloneMode(Column column)
		{
			string cloneMode = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "CloneMode");
			if (string.IsNullOrEmpty(cloneMode))
			{
				return CloneMode.Shallow;
			}
			CloneMode result = (CloneMode)Enum.Parse(typeof(CloneMode), cloneMode, true);
			return result;
		}
		#endregion

		#region GetColumnDefaultValueText
		/// <summary>
		/// Vrátí hodnoty defualt na databázi.
		/// Ořezanou o uvozovky.
		/// </summary>
		public static string GetColumnDefaultValueText(Column column)
		{
			if (column.DefaultConstraint == null)
			{
				return null;
			}

			string defaultText = column.DefaultConstraint.Text;
			while (defaultText.StartsWith("(") && defaultText.EndsWith(")"))
			{
				defaultText = defaultText.Substring(1, defaultText.Length - 2);
			}
			return defaultText;
		}
		#endregion

		#region GetDefaultValueExpression
		/// <summary>
		/// Vrátí kód reprezentující výchozí hodnotu pro daný sloupec.
		/// Hodnota se zjišťuje postupně z extended property na sloupci, poté z výchozí hodnoty na sloupci v databázi, dále z extended property v databázi. Není-li výchozí hodnota uvedena, vrací se kód pro výchozí hodnotu daného datového typu.
		/// Metoda vždy vrací nějaký text, nikdy null ani prázdný řetězec.
		/// </summary>
		public static string GetDefaultValueExpression(Column column)
		{
			string result = null;

			// pokud máme výchozí hodnotu pro sloupec v extended property sloupce, použijeme ji
			string defaultValueByColumnExtendedProperties = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "DefaultValue");
			if (!String.IsNullOrEmpty(defaultValueByColumnExtendedProperties))
			{
				result = defaultValueByColumnExtendedProperties;
			}

			// jinak se podíváme po default hodnotě na sloupci
			if (result == null)
			{
				string defaultValueByColumn = GetDefaultValueExpressionByDatabaseColumn(column);
				if (!String.IsNullOrEmpty(defaultValueByColumn))
				{
					result = defaultValueByColumn;
				}
			}

			// není-li hodnota ani v extended propery sloupce, ani na sloupci samotném (v databázi), zkusíme použít hodnotu na databázi
			// pokud máme výchozí hodnotu pro sloupec v extended property sloupce, použijeme ji			
			if (result == null)
			{
				string defaultValueByDatabaseExtendedProperties = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "DefaultValue_" + PropertyHelper.GetPropertyName(column));
				if (!String.IsNullOrEmpty(defaultValueByDatabaseExtendedProperties))
				{
					result = defaultValueByDatabaseExtendedProperties;
				}
			}

			// jinak výchozí hodnota není, vracíme proto text null
			if (result == null)
			{
				result = GetDefaultValueExpressionForUnspecifiedDefaultValue(column);
			}

			if (PropertyHelper.IsString(column) && String.Equals(result, "\"\""))
			{
				return "String.Empty";
			}

			return result;
		}
		#endregion

		#region GetDefaultValueExpressionByDatabaseColumn
		/// <summary>
		/// Vrací výchozí hodnotu dle databázového sloupce. Pokud není výchozí hodnota uvedena, vrací null.
		/// Použito v GetDefaultValueExpression.
		/// </summary>
		private static string GetDefaultValueExpressionByDatabaseColumn(Column column)
		{
			if (column.DefaultConstraint != null)
			{
				string cast = "";
				if (TypeHelper.IsNonstandardType(column))
				{
					cast = "(" + TypeHelper.GetPropertyTypeName(column) + ")";
				}

				string defaultValue = column.DefaultConstraint.Text;
				string trimmedQuotesDefaultValue = defaultValue.TrimStart('(');
				trimmedQuotesDefaultValue = trimmedQuotesDefaultValue.Substring(0, trimmedQuotesDefaultValue.Length - (defaultValue.Length - trimmedQuotesDefaultValue.Length));

				if (column.DataType.SqlDataType == SqlDataType.Bit)
				{
					if (trimmedQuotesDefaultValue == "0")
					{
						return "false";
					}

					if (trimmedQuotesDefaultValue == "1")
					{
						return "true";
					}
				}

				if (PropertyHelper.IsString(column))
				{
					if (defaultValue.StartsWith("('") && defaultValue.EndsWith("')"))
					{
						defaultValue = defaultValue.Substring(2, defaultValue.Length - 4);
					}

					if (defaultValue.StartsWith("(N'") && defaultValue.EndsWith("')"))
					{
						defaultValue = defaultValue.Substring(3, defaultValue.Length - 5);
					}

					if (!String.IsNullOrEmpty(defaultValue))
					{
						return String.Format("\"{0}\"", defaultValue.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("''", "'"));
					}

					return "String.Empty";
				}

				if (TypeHelper.IsDateTime(column))
				{
					if (trimmedQuotesDefaultValue.ToLower() == "getdate()")
					{
						return ApplicationTimeHelper.GetApplicationCurrentTimeExpression();
					}
				}

				if (TypeHelper.IsNumeric(column) && !TypeHelper.IsBusinessObjectReference(column))
				{
					string result = String.Format("{0}{1}", cast, trimmedQuotesDefaultValue);

					if (PropertyHelper.IsDecimal(column))
					{
						result += "M";
					}

					return result;
				}

				if ((column.DataType.SqlDataType == SqlDataType.UniqueIdentifier) && (defaultValue.ToLower() == "(newid())"))
				{
					return "Guid.NewGuid()";
				}
			}
			return null;
		}
		#endregion

		#region GetDefaultValueExpressionForUnspecifiedDefaultValue
		/// <summary>
		/// Vrací výchozí hodnotu pro daný typ databázového sloupce.
		/// Použito v GetDefaultValueExpression.
		/// /// </summary>
		private static string GetDefaultValueExpressionForUnspecifiedDefaultValue(Column column)
		{
			if ((PropertyHelper.IsString(column)))
			{
				return "String.Empty";
			}

			if (TypeHelper.ToSqlDbType(column.DataType) == SqlDbType.DateTime && !column.Nullable)
			{
				return "SqlDateTime.MinValue.Value";
			}

			if (TypeHelper.ToSqlDbType(column.DataType) == SqlDbType.SmallDateTime && !column.Nullable)
			{
				return "Havit.Data.SqlTypes.SqlSmallDateTime.MinValue.Value";
			}

			if ((column.Nullable || TypeHelper.IsBusinessObjectReference(column)))
			{
				return "null";
			}

			return String.Format("default({0})", TypeHelper.GetPropertyTypeName(column));
		}
		#endregion
	}
}
