using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.IndexBuilders;

public static class IndexesBuilder
{
	/// <summary>
	/// Vytvoří indexy k tabulce.
	/// </summary>
	public static void CreateIndexes(Table table)
	{
		if (GeneratorSettings.Strategy != GeneratorStrategy.Havit)
		{
			return;
		}

		if (TableHelper.GetGenerateIndexes(table))
		{
			List<string> generatorIndexes = new List<string>();

			if (TableHelper.IsJoinTable(table))
			{
				CreateJoinTableIndexes(table, generatorIndexes);
			}
			else
			{
				CreateNormalTableIndexes(table, generatorIndexes);
			}

			RemoveOldIndexes(table, generatorIndexes);
		}
	}

	/// <summary>
	/// Vytvoří indexy k tabulce, která je spojkou (realizuje vztah M:N).
	/// </summary>
	private static void CreateJoinTableIndexes(Table table, List<string> generatorIndexes)
	{
		Column column1 = table.Columns[0];
		Column column2 = table.Columns[1];

		CreateJoinTableIndex(table, column1, column2, generatorIndexes);
		CreateJoinTableIndex(table, column2, column1, generatorIndexes);
	}

	/// <summary>
	/// Vytvoří indexy k tabulce, která je spojkou (realizuje vztah M:N) pro sloupce v daném pořadí.
	/// </summary>
	private static void CreateJoinTableIndex(Table table, Column column1, Column column2, List<string> generatorIndexes)
	{
		string indexName = IndexHelper.GetIndexName("FKX", table.Name, column1.Name, column2.Name);

		Index index = new Index(table, indexName);
		index.IsUnique = true;

		IndexedColumn indexedColumn1 = new IndexedColumn(index, column1.Name);
		indexedColumn1.IsIncluded = false;

		IndexedColumn indexedColumn2 = new IndexedColumn(index, column2.Name);
		indexedColumn2.IsIncluded = false;

		index.IndexedColumns.Add(indexedColumn1);
		index.IndexedColumns.Add(indexedColumn2);

		Index currentIndex = IndexHelper.FindByStructure(table, index);
		if (currentIndex == null)
		{
			IndexHelper.CreateOrUpdate(table, index, generatorIndexes);
		}
		else
		{
			generatorIndexes.Add(index.Name); // pokud kritérium splnil jiný index, tak jej zapíšeme do seznamu generátorem podporovaných dotazů
		}
	}

	private static void CreateNormalTableIndexes(Table table, List<string> generatorIndexes)
	{
		foreach (Column column in TableHelper.GetNotIgnoredColumns(table))
		{
			if (!column.InPrimaryKey && column.IsForeignKey)
			{
				CreateColumnIndex(table, column, generatorIndexes);
			}
		}

		foreach (CollectionProperty collection in TableHelper.GetCollectionsTargetingTo(table))
		{
			CreateCollectionOrderIndex(table, collection, generatorIndexes);
		}

		if (LocalizationHelper.IsLocalizationTable(table))
		{
			CreateLocalizationIndex(table, generatorIndexes);
		}

		if (LanguageHelper.IsLanguageTable(table))
		{
			CreateLanguageIndex(table, generatorIndexes);
		}

	}

	private static void CreateColumnIndex(Table table, Column column, List<string> generatorIndexes)
	{
		if (ColumnHelper.GetGenerateIndexes(column))
		{
			Column deletedColumn = TableHelper.GetDeletedColumn(table);

			string indexName;
			if (deletedColumn == null)
			{
				indexName = IndexHelper.GetIndexName("FKX", table.Name, column.Name);
			}
			else
			{
				indexName = IndexHelper.GetIndexName("FKX", table.Name, column.Name, deletedColumn.Name);
			}

			Index index = new Index(table, indexName);
			index.IsUnique = false;

			IndexedColumn indexedColumn = new IndexedColumn(index, column.Name);
			indexedColumn.IsIncluded = false;
			index.IndexedColumns.Add(indexedColumn);

			if (deletedColumn != null)
			{
				IndexedColumn indexedDeletedColumn = new IndexedColumn(index, deletedColumn.Name);
				indexedDeletedColumn.IsIncluded = false;
				index.IndexedColumns.Add(indexedDeletedColumn);
			}

			//if (!column.InPrimaryKey)
			//{
			//	Column primaryKeyColumn = TableHelper.GetPrimaryKey(table);
			//	IndexedColumn indexedPrimaryKeyColumn = new IndexedColumn(index, primaryKeyColumn.Name);
			//	indexedPrimaryKeyColumn.IsIncluded = true;
			//	index.IndexedColumns.Add(indexedPrimaryKeyColumn);
			//}

			IndexHelper.CreateOrUpdate(table, index, generatorIndexes);
		}
	}

	private static void CreateCollectionOrderIndex(Table table, CollectionProperty collectionProperty, List<string> generatorIndexes)
	{
		if (collectionProperty.IsManyToMany)
		{
			return; // Many to many kolekce se ptá na join spojovací tabulky a primárního klíče, ev. se sloupcem deleted.
		}

		if (collectionProperty.IsOneToMany && (!String.IsNullOrEmpty(collectionProperty.Sorting)))
		{
			List<string> orderByColumns = new List<string>();
			MatchCollection matches = Regex.Matches(collectionProperty.Sorting, "(^|[^{]){([^{}]*)}");
			foreach (Match match in matches)
			{
				orderByColumns.Add(match.Groups[2].Value);
			}

			if (orderByColumns.Count == 0)
			{
				return;
			}

			if ((orderByColumns.Count == 1) && (table.FindColumn(orderByColumns[0]).IsForeignKey || table.FindColumn(orderByColumns[0]).InPrimaryKey))
			{
				// obsahuje právě jeden index a ten je primárním nebo cizím klíčem, pak je pokryto jiným indexem
				return;
			}

			orderByColumns.Remove(collectionProperty.ReferenceColumn.Name);
			orderByColumns.Insert(0, collectionProperty.ReferenceColumn.Name);

			string indexName = IndexHelper.GetIndexName("FKX", table.Name, orderByColumns.ToArray());
			Index index = new Index(table, indexName);
			index.IsUnique = false;

			foreach (string orderByColumn in orderByColumns)
			{
				IndexedColumn indexedColumn = new IndexedColumn(index, orderByColumn);
				indexedColumn.IsIncluded = false;
				index.IndexedColumns.Add(indexedColumn);
			}

			Column deletedColumn = TableHelper.GetDeletedColumn(table);
			if ((deletedColumn != null) && (!orderByColumns.Contains(deletedColumn.Name)))
			{
				IndexedColumn indexedColumn = new IndexedColumn(index, deletedColumn.Name);
				indexedColumn.IsIncluded = true;
			}

			IndexHelper.CreateOrUpdate(table, index, generatorIndexes);
		}

	}

	private static void CreateLocalizationIndex(Table table, List<string> generatorIndexes)
	{
		Column parentLocalizationColumn = LocalizationHelper.GetParentLocalizationColumn(table);
		Column languageColumn = LocalizationHelper.GetLanguageColumn(table);

		string indexName = IndexHelper.GetIndexName("FKX", table.Name, parentLocalizationColumn.Name, languageColumn.Name);
		Index index = new Index(table, indexName);
		index.IsUnique = true;

		IndexedColumn indexedParentLocalizationColumn = new IndexedColumn(index, parentLocalizationColumn.Name);
		indexedParentLocalizationColumn.IsIncluded = false;

		IndexedColumn indexedLanguageColumn = new IndexedColumn(index, languageColumn.Name);
		indexedLanguageColumn.IsIncluded = false;

		//IndexedColumn indexedPrimaryKeyColumn = new IndexedColumn(index, primaryKeyColumn.Name);
		//indexedPrimaryKeyColumn.IsIncluded = true;

		index.IndexedColumns.Add(indexedParentLocalizationColumn);
		index.IndexedColumns.Add(indexedLanguageColumn);
		//			index.IndexedColumns.Add(indexedPrimaryKeyColumn);

		IndexHelper.CreateOrUpdate(table, index, generatorIndexes);
	}

	private static void CreateLanguageIndex(Table table, List<string> generatorIndexes)
	{
		Column uiCultureColumn = LanguageHelper.GetUICultureColumn();
		if (uiCultureColumn != null)
		{
			string indexName = IndexHelper.GetIndexName("FKX", table.Name, uiCultureColumn.Name);
			Index index = new Index(table, indexName);
			index.IsUnique = true;

			IndexedColumn indexedIiCultureColumnName = new IndexedColumn(index, uiCultureColumn.Name);
			indexedIiCultureColumnName.IsIncluded = false;

			index.IndexedColumns.Add(indexedIiCultureColumnName);
			IndexHelper.CreateOrUpdate(table, index, generatorIndexes);
		}
	}

	private static void RemoveOldIndexes(Table table, List<string> generatorIndexes)
	{
		List<string> currentGeneratorIndexes = new List<string>();
		foreach (Index index in table.Indexes)
		{
			if (index.Name.StartsWith("FKX_"))
			{
				currentGeneratorIndexes.Add(index.Name);
			}
		}

		List<string> indexesToRemove = currentGeneratorIndexes.Except(generatorIndexes).ToList();
		foreach (string indexToRemove in indexesToRemove)
		{
			table.Indexes[indexToRemove].Drop();
		}

	}
}
