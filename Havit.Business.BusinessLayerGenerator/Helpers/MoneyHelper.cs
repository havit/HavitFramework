using System;
using System.Collections.Generic;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class MoneyHelper
	{
		/// <summary>
		/// Vrátí název fieldu pro danou property.
		/// </summary>
		public static string GetMoneyFieldName(string moneyProperty)
		{
			return ConventionsHelper.GetUnderScoped(ConventionsHelper.GetCammelCase(moneyProperty));
		}

		/// <summary>
		/// Vrátí jméno typu, které má být použito pro datový typ Money.
		/// </summary>
		public static string GetMoneyTypeName()
		{
			Table currencyTable = DatabaseHelper.FindTable("Currency", "dbo");
			string result = NamespaceHelper.GetNamespaceName(currencyTable);
			if (result != String.Empty)
			{
				result += ".";
			}
			result += "Money";
			return result;
		}

		/// <summary>
		/// Vrátí název typu pro měnu.
		/// </summary>
		public static Table GetCurrencyTable()
		{
			Table currencyTable = DatabaseHelper.FindTable("Currency", "dbo");
			if ((currencyTable != null) && TableHelper.IsIgnored(currencyTable))
			{
				currencyTable = null;
			}
			return currencyTable;
		}

		/// <summary>
		/// Vrátí true, pokud daný sloupec tvoří složenou vlastnost typu Měna.
		/// </summary>
		public static bool FormsMoneyStructure(Column column)
		{
			string shortenedColumnName = ShortcutColumnNameToMoneyPropertyName(column.Name);

			if (!String.IsNullOrEmpty(shortenedColumnName))
			{
				Table table = (Table)column.Parent;
				return ExistMoney(table, shortenedColumnName);
			}

			return false;
		}

		/// <summary>
		/// Vrátí název property pro vlastnost typu Money z daného sloupce. Pokud daný sloupec nemůže být 
		/// </summary>
		public static string ShortcutColumnNameToMoneyPropertyName(string columnName)
		{
			string result = null;
			if (columnName.EndsWith("Amount"))
			{
				result = columnName.Substring(0, columnName.Length - "Amount".Length);
			}

			if (columnName.EndsWith("CurrencyID"))
			{
				result = columnName.Substring(0, columnName.Length - "CurrencyID".Length);
			}

			if (result == String.Empty)
			{
				result = null;
			}

			return result;
		}

		/// <summary>
		/// Vrací true, pokud v tabulce existuje dvojice sloupců určující vlastnost typu měna daného jména.
		/// </summary>
		public static bool ExistMoney(Table table, string name)
		{
			Column amountColumn = table.Columns[name + "Amount"];
			Column currencyColumn = table.Columns[name + "CurrencyID"];

			if ((amountColumn == null) || (currencyColumn == null))
			{
				return false;
			}

			if (ColumnHelper.IsIgnored(amountColumn) || ColumnHelper.IsIgnored(currencyColumn))
			{
				return false;
			}

			if (TypeHelper.IsNumeric(amountColumn) && TypeHelper.IsBusinessObjectReference(currencyColumn))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Vrátí seznam struktur typu Money, které mají ve třídě k dané tabulce existovat.
		/// </summary>
		public static string[] GetListMoneyProperties(Table table)
		{
			List<string> result = new List<string>();

			foreach (Column column in table.Columns.SortIfNecessary())
			{
				if (!ColumnHelper.IsIgnored(column))
				{
					string shortcutName = ShortcutColumnNameToMoneyPropertyName(column.Name);
					if ((shortcutName != null) && (!result.Contains(shortcutName)))
					{
						if (ExistMoney(table, shortcutName))
						{
							result.Add(shortcutName);
						}
					}
				}
			}
			return result.ToArray();
		}

		/// <summary>
		/// Vrátí sloupec pro částku dané property typu Money.
		/// </summary>
		public static Column GetMoneyAmountColumn(Table table, string moneyPropertyName)
		{
			Column column = table.Columns[moneyPropertyName + "Amount"];
			if ((column != null) && ColumnHelper.IsIgnored(column))
			{
				column = null;
			}
			return column;
		}

		/// <summary>
		/// Vrátí sloupec pro měnu dané property typu Money.
		/// </summary>
		public static Column GetMoneyCurrencyColumn(Table table, string moneyPropertyName)
		{
			Column column = table.Columns[moneyPropertyName + "CurrencyID"];
			if ((column != null) && ColumnHelper.IsIgnored(column))
			{
				column = null;
			}
			return column;
		}
	}
}
