using System;
using System.Data;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class RulesChecker
{
	/// <summary>
	/// Ověří pravidla nad tabulkou.
	/// </summary>
	public static void CheckRules(Table table)
	{
		CheckPrimaryKeyNamingRules(table);
		CheckPrimaryIsAutoincrementRule(table);
		CheckAccessModifier(table);
		CheckDescriptionRules(table);
		CheckDeprecatedTypes(table);
		CheckCacheExtendedPropertiesRule(table);
		CheckPrimaryKeyHasIndexRule(table);
		CheckForeignKeysNamingConvention(table);
		CheckDeletedColumnDataTypeRule(table);
		CheckCreatedColumnDataTypeRule(table);
		CheckDefaultValueRule(table);
		CheckPropertyAccessorsRule(table);
		CheckLocalizationRules(table);
		CheckCollectionForeignKeyRules(table);
		CheckMoneyRules(table);
		CheckResourceClassRules(table);
		CheckResourceItemRules(table);

		StoredProcedureRulesChecker.CheckRules(table);
	}

	/// <summary>
	/// Ověří, že název sloupce, který je cizím klíčem, končí na "ID".
	/// </summary>
	private static void CheckPrimaryKeyNamingRules(Table table)
	{
		if ((GeneratorSettings.Strategy != GeneratorStrategy.Havit) && (GeneratorSettings.Strategy != GeneratorStrategy.HavitCodeFirst))
		{
			return;
		}

		Column primaryKeyColumn = TableHelper.GetPrimaryKey(table);

		if (primaryKeyColumn.Name == table.Name)
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: Název primárního klíče nekončí na \"ID\".", table.Name);
		}
		else if (primaryKeyColumn.Name != (table.Name + "ID"))
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: Název primárního klíče musí být pojmenován {0}ID.", table.Name);
		}

	}

	/// <summary>
	/// Ověřuje, zda má tabulka primární klíč autoincrement, pokud není readonly. Pokud nemá, zapisuje do chybové konzole.
	/// Při strategii pro Exec netestuje nic.
	/// </summary>
	public static void CheckPrimaryIsAutoincrementRule(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return;
		}

		var primaryKeyColumn = TableHelper.GetPrimaryKey(table);

		if (!primaryKeyColumn.Identity && String.IsNullOrEmpty(primaryKeyColumn.DefaultConstraint?.Text) && !TableHelper.IsReadOnly(table) && !TableHelper.OmitCreateObjectMethod(table))
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: Primární klíč není autoincrement a nemá default (a není zakázána metoda CreateObject a tabulka není readonly).", table.Name);
		}
	}

	/// <summary>
	/// Ověřuje, zda má tabulka korektně nastaven AccessModifier. Pokud nemá, zapisuje do chybové konzole.
	/// </summary>
	public static void CheckAccessModifier(Table table)
	{
		string accessModifier = TableHelper.GetAccessModifier(table);
		string[] allowedAccessModifiers = new string[] { "public", "internal" };
		if (!allowedAccessModifiers.Contains(accessModifier))
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: AccessModifier má neznámou hodnotu. Povoleno je public a internal.", table.Name);
		}
	}

	/// <summary>
	/// Ověřuje, zda má tabulka a všechny sloupečky, ze kterých budou generovány vlastnosti, popis (description),
	/// který slouží jako komentář v C# kódu.
	/// </summary>
	public static void CheckDescriptionRules(Table table)
	{
		if (String.IsNullOrEmpty(TableHelper.GetDescription(table)))
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: Chybí popis (description).", table.Name);
		}

		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			if (!String.IsNullOrEmpty(ColumnHelper.GetDescription(column)))
			{
				continue;
			}

			if (MoneyHelper.FormsMoneyStructure(column) && (column == MoneyHelper.GetMoneyCurrencyColumn(table, MoneyHelper.ShortcutColumnNameToMoneyPropertyName(column.Name))))
			{
				continue;
			}

			if (String.IsNullOrEmpty(ExtendedPropertiesHelper.GetDescription(ExtendedPropertiesKey.FromColumn(column))))
			{
				ConsoleHelper.WriteLineWarning("Tabulka {0}, Sloupec {1}: Chybí popis (description).", table.Name, column.Name);
			}

		}

		foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
		{
			if (String.IsNullOrEmpty(collectionProperty.Description))
			{
				ConsoleHelper.WriteLineWarning("Tabulka {0}, Kolekce {1}: Chybí popis (description).", table.Name, collectionProperty.PropertyName);
			}
		}
	}

	/// <summary>
	/// Ověřuje, zda pro některý sloupeček není použit zastaralý datový typ.
	/// Při strategii pro Exec netestuje nic.
	/// </summary>
	public static void CheckDeprecatedTypes(Table table)
	{
		if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
		{
			return;
		}

		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			if (ColumnHelper.IsDeprecatedType(column))
			{
				ConsoleHelper.WriteLineWarning("Tabulka {0}, Sloupec {1}: Použit zastaralý typ {2}.", table.Name, column.Name, column.DataType.SqlDataType.ToString());
			}
		}
	}

	/// <summary>
	/// Ověří pravidla extended properties.
	/// Nyní se testuje, zda nejsou přítomny hodnoty pro cache - absolute expiration a sliding expiration - zároveň.
	/// </summary>
	public static void CheckCacheExtendedPropertiesRule(Table table)
	{
		if (TableHelper.GetCacheAbsoluteExpirationSeconds(table) != null && TableHelper.GetCacheSlidingExpirationSeconds(table) != null)
		{
			ConsoleHelper.WriteLineWarning("Tabulka {0}: Nastaveno Cache_AbsolutniExpiration i Cache_SlidingExpiration.", table.Name);
		}
	}

	/// <summary>
	/// Ověří, zda má tabulka s autoincrement PK clusterovaný index.
	/// </summary>
	public static void CheckPrimaryKeyHasIndexRule(Table table)
	{
		if (GeneratorSettings.Strategy != GeneratorStrategy.Exec)
		{
			Column primaryKey = TableHelper.GetPrimaryKey(table);

			foreach (DataRow dataRow in primaryKey.EnumIndexes().Rows)
			{
				Index index = table.Indexes[(string)dataRow[0]];
				if (index.IndexedColumns.Count == 1)
				{
					if (primaryKey.Identity && !index.IsClustered)
					{
						ConsoleHelper.WriteLineError(String.Format("Tabulka {0}: Primární klíč je autoincrement, ale index primárního klíče není clustrovaný.", table.Name));
					}
					return;
				}
			}
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}: Nenalezen index primárního klíče.", table.Name));
		}
	}

	/// <summary>
	/// Ověří, že název sloupce, který je cizím klíčem, končí na "ID".
	/// </summary>
	private static void CheckForeignKeysNamingConvention(Table table)
	{
		if ((GeneratorSettings.Strategy != GeneratorStrategy.Havit) && (GeneratorSettings.Strategy != GeneratorStrategy.HavitCodeFirst))
		{
			return;
		}

		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			if (ColumnHelper.CheckForeignKeyName(column))
			{
				if (column.IsForeignKey && !column.Name.EndsWith("ID", StringComparison.CurrentCulture))
				{
					ConsoleHelper.WriteLineWarning("Tabulka {0}, Sloupec {1}: Sloupec je cizím klíčem, ale název nekončí \"ID\".", table.Name, column.Name);
				}

				if (!column.IsForeignKey && column.Name.EndsWith("ID", StringComparison.CurrentCulture))
				{
					ConsoleHelper.WriteLineWarning("Tabulka {0}, Sloupec {1}: Sloupec není cizím klíčem, ale název končí \"ID\".", table.Name, column.Name);
				}
			}
		}
	}

	/// <summary>
	/// Ověří datový typ sloupce, který se jmenuje "Deleted". Očekává se bit, datetime nebo smalldatetime.
	/// </summary>
	public static void CheckDeletedColumnDataTypeRule(Table table)
	{
		if ((table.Columns["Deleted"] != null) && (TableHelper.GetDeletedColumn(table) == null))
		{
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec Deleted: Očekáván datový typ Bit, DateTime nebo SmallDateTime.", table.Name));
		}
		Column deletedColumn = TableHelper.GetDeletedColumn(table);

		if ((deletedColumn != null) && (deletedColumn.DataType.SqlDataType == SqlDataType.Bit))
		{
			if (deletedColumn.Nullable)
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec Deleted: Povolena hodnota null sloupce typu bit.", table.Name));
			}
		}
	}

	/// <summary>
	/// Ověří existenci datový typ sloupce "Created" a existenci výchozí hodnoty.
	/// </summary>
	private static void CheckCreatedColumnDataTypeRule(Table table)
	{
		Column createdColumn = table.Columns["Created"];
		if (createdColumn != null)
		{
			if (!PropertyHelper.IsDateTime(createdColumn))
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec Created: Datový typ není DateTime.", table.Name));
			}
			else
			{
				bool columnDefaultExists = (createdColumn.DefaultConstraint != null) && (!String.IsNullOrEmpty(createdColumn.DefaultConstraint.Text));
				bool extendedPropertyDefaultExists = !String.IsNullOrEmpty(ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(createdColumn), "DefaultValue"));
				bool databaseExtendedPropertyDefaultExists = !String.IsNullOrEmpty(ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "DefaultValue_" + PropertyHelper.GetPropertyName(createdColumn)));

				if (!columnDefaultExists && !extendedPropertyDefaultExists && !databaseExtendedPropertyDefaultExists)
				{
					ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec {1}: Chybí výchozí hodnota.", table.Name, createdColumn.Name));
				}
			}

		}
	}

	private static void CheckDefaultValueRule(Table table)
	{
		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			bool columnDefaultExists = (column.DefaultConstraint != null) && (!String.IsNullOrEmpty(column.DefaultConstraint.Text));
			bool extendedPropertyDefaultExists = !String.IsNullOrEmpty(ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "DefaultValue"));
			if (columnDefaultExists && extendedPropertyDefaultExists)
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec {1}: Je definována dvojí výchozí hodnota na sloupci: Na sloupci samotném i jako extended property.", table.Name, column.Name));
			}
		}
	}

	/// <summary>
	/// Ověřuje zda není uvedena rozšířená vlastnost GetAccessot a SetAccessor současně.
	/// </summary>
	private static void CheckPropertyAccessorsRule(Table table)
	{
		foreach (Column column in TableHelper.GetPropertyColumns(table))
		{
			if (!String.IsNullOrEmpty(PropertyHelper.GetPropertyGetterAccessModifier(column)) && !String.IsNullOrEmpty(PropertyHelper.GetPropertySetterAccessModifier(column)))
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec {1}: Uvedeny současně GetAccessor a SetAccessor.", table.Name, column.Name));
			}
		}
	}

	/// <summary>
	/// Pokud jde o lokalizační tabulku, ověřuje pravidla pro tuto tabulku.
	/// </summary>
	private static void CheckLocalizationRules(Table table)
	{
		if (!LocalizationHelper.IsLocalizationTable(table))
		{
			return;
		}

		Column parentColumn = LocalizationHelper.GetParentLocalizationColumn(table);
		Column languageColumn = table.Columns[LocalizationHelper.LanguageForeignKeyColumnName];

		if (parentColumn == null)
		{
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}: Nenalezen sloupec odkazující na lokalizovanou tabulku.", table.Name));
			return;
		}

		if (languageColumn == null)
		{
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}: Nenalezen sloupec odkazující na tabulku jazyků.", table.Name));
			return;
		}

		if (!TypeHelper.IsBusinessObjectReference(parentColumn))
		{
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec {1}: Není nastavena reference.", table.Name, parentColumn.Name));
		}

		if (!TypeHelper.IsBusinessObjectReference(languageColumn))
		{
			ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}, Sloupec LanguageID: Není nastavena reference.", table.Name));
		}
	}

	/// <summary>
	/// Ověří, zda existuje cizí klíč odpovídající kolekci.
	/// </summary>
	private static void CheckCollectionForeignKeyRules(Table table)
	{
		foreach (CollectionProperty collectionProperty in TableHelper.GetCollectionColumns(table))
		{
			ForeignKey referenceColumnForeignKey = (collectionProperty.JoinTable ?? collectionProperty.TargetTable).ForeignKeys.AsEnumerable().FirstOrDefault(fk => fk.Columns.Count == 1 && fk.Columns.Contains(collectionProperty.ReferenceColumn.Name));
			if ((referenceColumnForeignKey != null)
				&& (referenceColumnForeignKey.ReferencedTable == collectionProperty.ParentTable.Name) /* collectionProperty.ParentTable === table */
				&& (referenceColumnForeignKey.ReferencedTableSchema == collectionProperty.ParentTable.Schema))
			{
				// OK
			}
			else
			{
				ConsoleHelper.WriteLineWarning("Tabulka {0}, Sloupec {1}: Chybí odpovídající cizí klíč z tabulky {2}, sloupce {3}. Zkontrolujte, zda je kolekce nastavena správně.",
					table.Name,
					collectionProperty.PropertyName,
					(collectionProperty.JoinTable ?? collectionProperty.TargetTable).Name,
					collectionProperty.ReferenceColumn.Name);
			}
		}
	}

	/// <summary>
	/// Zkontroluje pravidla pro sloupce typu Money.
	/// </summary>
	private static void CheckMoneyRules(Table table)
	{
		string[] moneyProperties = MoneyHelper.GetListMoneyProperties(table);
		foreach (string moneyProperty in moneyProperties)
		{
			Column amountColumn = MoneyHelper.GetMoneyAmountColumn(table, moneyProperty);
			Column currencyColumn = MoneyHelper.GetMoneyCurrencyColumn(table, moneyProperty);

			if (amountColumn.Nullable != currencyColumn.Nullable)
			{
				ConsoleHelper.WriteLineWarning(String.Format("Tabulka {0}: Sloupce {1} a {2} musí být oba nullable nebo oba not-null.", table.Name, amountColumn.Name, currencyColumn.Name));
			}
		}
	}

	private static void CheckResourceClassRules(Table table)
	{
		if (table.Name == "ResourceClass")
		{
			if (table.Columns["Nazev"] != null)
			{
				ConsoleHelper.WriteLineWarning("Tabulka ResourceClass: Sloupec Nazev by měl být přejmenován na Name.");
			}

			if (table.Columns["Popis"] != null)
			{
				ConsoleHelper.WriteLineWarning("Tabulka ResourceClass: Sloupec Popis by měl být přejmenován na Description.");
			}
		}
	}

	private static void CheckResourceItemRules(Table table)
	{
		if (table.Name == "ResourceItem")
		{
			if (table.Columns["Popis"] != null)
			{
				ConsoleHelper.WriteLineWarning("Tabulka ResourceItem: Sloupec Popis by měl být přejmenován na Description.");
			}
		}
	}
}
