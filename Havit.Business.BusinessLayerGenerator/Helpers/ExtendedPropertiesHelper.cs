using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
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
                SqlCommand cmd = new SqlCommand("SELECT class_desc, major_id, minor_id, name, value FROM sys.extended_properties");

                List<Tuple<ExtendedPropertiesKey, string, string>> data = new List<Tuple<ExtendedPropertiesKey, string, string>>();
                using (SqlDataReader dataReader = ConnectionHelper.GetDataReader(cmd))
                {
                    while (dataReader.Read())
                    {
                        string classDesc = dataReader.GetString(0);
                        long majorId = dataReader.GetInt32(1);
                        long minorId = dataReader.GetInt32(2);
                        string name = (string)dataReader["name"];
                        object value = dataReader["value"];

                        if (value is string)
                        {
                            name = name.Replace("_", ".");
                            var key = new ExtendedPropertiesKey(classDesc, majorId, minorId);
                            data.Add(new Tuple<ExtendedPropertiesKey, string, string>(key, name, (string)value));
                        }
                    }
                }
                allExtendedProperties = data.GroupBy(item => item.Item1 /* Key */).ToDictionary(group => group.Key, group => group.ToDictionary(item => item.Item2 /* Name */, item => item.Item3 /* Value */, StringComparer.CurrentCultureIgnoreCase));

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
}
