using System;
using System.Collections;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions
{
	public static class ClassHelper
	{
		private static Hashtable typeNamesCache = new Hashtable();

		/// <summary>
		/// Vrací název třídy tabulky.
		/// </summary>
		public static string GetClassName(Table table)
		{
			string result = (string)typeNamesCache[table];

			// máme-li název v cache, použijeme jej
			if (result != null)
			{
				return result;
			}

			// zkusíme název z extended property TypeName
			result = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "TypeName");

			// pokud neexistuje existuje extended property, použujeme název databázové tabulky
			if (String.IsNullOrEmpty(result))
			{
				result = table.Name;

				if (result.EndsWith("_Lang"))
				{
					result = result.Replace("_Lang", "Localization");
				}
			}

			typeNamesCache[table] = result;
			return result;
		}

		/// <summary>
		/// Vrací název předka třídy tabulky.
		/// </summary>
		public static string GetBaseClassName(Table table)
		{
			return GetClassName(table) + "Base";
		}

		/// <summary>
		/// Vrací název třídy včetně namespace.
		/// </summary>
		public static string GetClassFullName(Table table, bool withDefaultNamespace = true)
		{
			return CombineNamespaceClassNames(NamespaceHelper.GetNamespaceName(table, withDefaultNamespace), GetClassName(table));
		}

		/// <summary>
		/// Vrátí jméno třídy pro kolekci objektů.
		/// </summary>
		public static string GetCollectionClassName(Table table)
		{
			return GetClassName(table) + "Collection";
		}

		/// <summary>
		/// Vrátí jméno třídy pro předka kolekce objektů.
		/// </summary>
		public static string GetCollectionBaseClassName(Table table)
		{
			return GetClassName(table) + "CollectionBase";
		}

		/// <summary>
		/// Vrátí jméno třídy pro kolekci objektů.
		/// </summary>
		public static string GetCollectionClassFullName(Table table, bool withDefaultNamespace = true)
		{
			return CombineNamespaceClassNames(NamespaceHelper.GetNamespaceName(table, withDefaultNamespace), GetClassName(table) + "Collection");
		}

		/// <summary>
		/// Vrátí celý název třídy jako kombinaci namespace a třídy v daném namespace. 
		/// Pokud je namespace prázdný, vrací název třídy.
		/// </summary>
		private static string CombineNamespaceClassNames(string namespaceName, string className)
		{
			if (String.IsNullOrEmpty(namespaceName))
			{
				return className;
			}
			return String.Format("{0}.{1}", namespaceName, className);
		}

		/// <summary>
		/// Vrátí jméno třídy nesoucí Properties pro danou tabulku.
		/// </summary>
		public static string GetPropertiesClassName(Table table)
		{
			return GetClassName(table) + "Properties";
		}

		/// <summary>
		/// Vrátí jméno předka třídy nesoucí Properties pro danou tabulku.
		/// </summary>
		public static string GetPropertiesBaseClassName(Table table)
		{
			return GetPropertiesClassName(table) + "Base";
		}

		/// <summary>
		/// Vrací jméno předka business objektu pro danou tabulku.
		/// Pokud není nastaveno na tabulce, bere se default z databáze. Není-li ani na databázi,
		/// použije se výchozí "ActiveRecordBusinessObjectBase".
		/// </summary>
		public static string GetBusinessObjectBaseType(Table table)
		{
			string tableBaseType = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromTable(table), "BusinessObjectBaseType");
			if (!String.IsNullOrEmpty(tableBaseType))
			{
				return tableBaseType;
			}

			string layerSupertype = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "BusinessObjectLayerSupertype");
			if (!String.IsNullOrEmpty(layerSupertype))
			{
				return layerSupertype;
			}

			return "ActiveRecordBusinessObjectBase";

		}

		/// <summary>
		/// Vrací jméno layer supertype kolekce business objektů.
		/// Výchozí hodnota je "BusinessObjectCollection&lt;business object&gt;".
		/// </summary>
		public static string GetBusinessObjectLayerCollectionSupertype(Table table)
		{
			string layerSupertype = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "BusinessObjectCollectionLayerSupertype");
			if (String.IsNullOrEmpty(layerSupertype))
			{
				layerSupertype = "BusinessObjectCollection<{BusinessObjectClassName}, {BusinessObjectCollectionClassName}>";
			}

			layerSupertype = layerSupertype.Replace("{BusinessObjectClassName}", ClassHelper.GetClassName(table));
			layerSupertype = layerSupertype.Replace("{BusinessObjectCollectionClassName}", ClassHelper.GetCollectionClassName(table));
			return layerSupertype;
		}
	}
}
