using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

//using Microsoft.SqlServer.Management.Nmo;

namespace Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions
{
	public static class PropertyHelper
	{
		#region Private fields
		private static Hashtable propertyNamesCache = new Hashtable();
		#endregion

		#region GetPropertyName
		/// <summary>
		/// Vrátí název pro property na základě jména sloupce.
		/// Pokud je sloupec cizím klíčema a končí na ID, je toto ID odebráno.
		/// Jinak vrací název sloupce.
		/// </summary>
		public static string GetPropertyName(Column column, string primaryKeyName = "ID")
		{
			string result = (string)propertyNamesCache[column];

			// máme-li název z cache, použijeme jej
			if (result != null)
			{
				return result;
			}

			if (column.InPrimaryKey)
			{
				// primární klíč má název property ID
				result = primaryKeyName;
			}
			else
			{
				// zkusíme, zda je u sloupce extended property
				result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "PropertyName");

				// pokud extended property není
				if (String.IsNullOrEmpty(result))
				{
					if (TypeHelper.IsBusinessObjectReference(column) && column.Name.StartsWith("ID_") && (GeneratorSettings.Strategy == GeneratorStrategy.WikiReality))
					{
						result = column.Name.Substring(3);
					}
					// ořízneme u cizího klíče z konce názvu sloupce ID, pokud tam je
					else if (TypeHelper.IsBusinessObjectReference(column) && column.Name.EndsWith("ID"))
					{
						result = column.Name.Substring(0, column.Name.Length - 2);
					}
					else
					// jinak použijeme název sloupce
					{
						result = column.Name;
					}

					if (GeneratorSettings.Strategy == GeneratorStrategy.Exec)
					{
						// ořízneme trigram
						result = result.Substring(3);
						// máme oříznuto ID na konci a trigram ze zařátku
						// pokud nám nic nezbylo...
						if ((result == "") && (TypeHelper.IsBusinessObjectReference(column)))
						{
							Table referencedTable = ColumnHelper.GetReferencedTable(column);
							if (referencedTable == null)
							{
								throw new ApplicationException(String.Format("Sloupec {0}: Obsahuje referenci na tabulku, která nebyla nalezena (Ignored?).", column.Name));
							}
							result = ClassHelper.GetClassName(referencedTable);
						}
					}
				}
			}

			propertyNamesCache[column] = result;
			return result;
		}
		#endregion

		#region GetPropertyHolderName
		/// <summary>
		/// Vrátí název pro property holder na základě názvu property
		/// </summary>
		public static string GetPropertyHolderName(string propertyName)
		{
			return String.Format("_{0}PropertyHolder", propertyName);
		}

		/// <summary>
		/// Vrátí název pro property holder na základě databázového sloupce
		/// </summary>
		public static string GetPropertyHolderName(Column column)
		{
			return GetPropertyHolderName(GetPropertyName(column));
		}
		#endregion

		#region IsString
		/// <summary>
		/// Vrátí true, pokud pro ukládání hodnoty ve sloupci je používát datový typ String.
		/// </summary>
		public static bool IsString(Column column)
		{
			return TypeHelper.GetPropertyTypeName(column) == "string";
		}
		#endregion

		#region IsDecimal
		/// <summary>
		/// Vrátí true, pokud pro ukládání hodnoty ve sloupci je používát datový typ Decimal.
		/// </summary>
		public static bool IsDecimal(Column column)
		{
			return TypeHelper.GetPropertyTypeName(column).StartsWith("Decimal");
		}
		#endregion

		#region IsDateTime
		/// <summary>
		/// Vrátí true, pokud pro ukládání hodnoty ve sloupci je používát datový typ DateTime.
		/// </summary>
		public static bool IsDateTime(Column column)
		{
			return TypeHelper.GetPropertyTypeName(column).StartsWith("DateTime");
		}
		#endregion

		#region IsBoolean
		/// <summary>
		/// Vrátí true, pokud pro ukládání hodnoty ve sloupci je používát datový typ bool.
		/// </summary>
		public static bool IsBoolean(Column column)
		{
			return TypeHelper.GetPropertyTypeName(column).StartsWith("bool");
		}
		#endregion

		#region GetPropertyAccessModifier
		/// <summary>
		/// Vrátí přístupový modifikátor pro getter property.
		/// </summary>
		public static string GetPropertyAccessModifier(Column column)
		{
			string modifier = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "PropertyAccessModifier");
			if (String.IsNullOrEmpty(modifier) && MoneyHelper.FormsMoneyStructure(column))
			{
				modifier = "protected";
			}

			if (String.IsNullOrEmpty(modifier))
			{
				if (TypeHelper.IsBusinessObjectReference(column))
				{
					Table targetTable = ColumnHelper.GetReferencedTable(column);
					if (TableHelper.GetAccessModifier(targetTable) == "internal")
					{
						modifier = "internal";
					}
				}
			}

			if (String.IsNullOrEmpty(modifier))
			{
				modifier = "public";
			}

			return modifier;
		}
		#endregion

		#region GetPropertyGetterAccessModifier
		/// <summary>
		/// Vrátí přístupový modifikátor pro getter property.
		/// </summary>
		public static string GetPropertyGetterAccessModifier(Column column)
		{
			string result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "GetAccessModifier");
			if (result == GetPropertyAccessModifier(column))
			{
				return "";
			}
			return result ?? String.Empty;
		}
		#endregion

		#region GetPropertySetterAccessModifier
		/// <summary>
		/// Vrátí přístupový modifikátor pro setter property.
		/// </summary>
		public static string GetPropertySetterAccessModifier(Column column)
		{
			string result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "SetAccessModifier");
			if ((result == null) && (column.Name == "Deleted"))
			{
				result = "protected";
			}
			if (result == GetPropertyAccessModifier(column))
			{
				return "";
			}
			return result ?? String.Empty;
		}
		#endregion

		#region ExistsPropertyInBaseType
		/// <summary>
		/// Vrací true, pokud property existuje v base třídě business objektu.
		/// Jde o podporu pro override.
		/// </summary>
		public static bool ExistsPropertyInBaseType(Table table, string propertyName)
		{
			string businessObjectBaseType = ClassHelper.GetBusinessObjectBaseType(table);
			string extendedPropertyName = String.Format("Type_{0}_Members", businessObjectBaseType);

			string members = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), extendedPropertyName);
			if (!String.IsNullOrEmpty(members))
			{
				string[] memberValues = members.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string memberValue in memberValues)
				{
					if ((memberValue != null) && (memberValue.Trim() == propertyName))
					{
						return true;
					}
				}
			}
			return false;
		}
		#endregion

		#region IsInternalDueCollectionClonning
		/// <summary>
		/// Vrací true, pokud je sloupec uveden v některé z kolekcí jako cíl reference kolekce (CollectionProperty.ReferenceColumn).
		/// </summary>
		public static bool IsInternalDueCollectionClonning(Column column)
		{
			if (_internalDueCollectionClonningColumns == null)
			{
				_internalDueCollectionClonningColumns =
					DatabaseHelper.GetWorkingTables()
					.SelectMany(table => TableHelper.GetCollectionColumns(table))
					.Where(item => item.IsOneToMany)
					.Where(item => TableHelper.GetCloneMethod(item.ParentTable))
					.Select(item => item.ReferenceColumn)
					.Distinct()
					.ToList();
			}
			return _internalDueCollectionClonningColumns.Contains(column);
		}
		private static List<Column> _internalDueCollectionClonningColumns;
		#endregion
	}
}
